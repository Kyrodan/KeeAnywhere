[CmdletBinding()]
param(
    [string]$KeePassDir,
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release",
    [switch]$Cleanup
)

$ErrorActionPreference = "Stop"
$repoRoot = $PSScriptRoot
Set-Location $repoRoot

# 1. Locate portable KeePass (or ask).
if (-not $KeePassDir) {
    $defaultDir = Join-Path (Split-Path $repoRoot -Parent) "KeePass-2.61"
    if (Test-Path (Join-Path $defaultDir "KeePass.exe")) {
        $KeePassDir = $defaultDir
    }
}

while (-not $KeePassDir -or -not (Test-Path (Join-Path $KeePassDir "KeePass.exe"))) {
    if ($KeePassDir) {
        Write-Host "KeePass.exe not found in '$KeePassDir'." -ForegroundColor Yellow
    } else {
        Write-Host "Portable KeePass not found." -ForegroundColor Yellow
        Write-Host "Download the portable ZIP from https://keepass.info/download.html and extract it anywhere."
    }
    $KeePassDir = Read-Host "Path to your portable KeePass folder"
    if ([string]::IsNullOrWhiteSpace($KeePassDir)) { throw "Cancelled." }
}

$kpExe = Join-Path $KeePassDir "KeePass.exe"
$pluginsDir = Join-Path $KeePassDir "Plugins"
if (-not (Test-Path $pluginsDir)) { New-Item -ItemType Directory -Path $pluginsDir | Out-Null }

# 2. Build (Release triggers PlgxTool's AfterBuild target).
$vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vswhere)) { throw "vswhere.exe not found. Install Visual Studio 2017+ or VS Build Tools." }
$msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
if (-not $msbuild) { throw "MSBuild not found via vswhere." }

Write-Host "Building $Configuration..." -ForegroundColor Cyan
& (Join-Path $repoRoot "nuget.exe") restore (Join-Path $repoRoot "KeeAnywhere.sln") | Out-Null
if ($LASTEXITCODE -ne 0) { throw "nuget restore failed." }
& $msbuild (Join-Path $repoRoot "KeeAnywhere\KeeAnywhere.csproj") "/p:Configuration=$Configuration" "/t:Clean,Build" "/nologo" "/v:q" | Out-Null
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

$plgx = Join-Path $repoRoot "KeeAnywhere\bin\$Configuration\KeeAnywhere.plgx"
if (-not (Test-Path $plgx)) { throw "$plgx wasn't produced. PlgxTool only fires on Release." }
Write-Host "Built: $plgx" -ForegroundColor Green

# 3. Make sure no KeePass instance is holding the Plugins folder.
& $kpExe --exit-all 2>&1 | Out-Null
Get-Process KeePass -ErrorAction SilentlyContinue | Wait-Process -Timeout 5 -ErrorAction SilentlyContinue

# 4. Optional cache reset. KeePass keeps a per-plgx-hash compiled DLL under
#    %LOCALAPPDATA%\KeePass\PluginCache; a stale entry can mask a real failure.
#    Off by default — pass -Cleanup to wipe it (and the deployed plgx).
if ($Cleanup) {
    Write-Host "Cleanup: wiping PluginCache and prior plgx deployment." -ForegroundColor Yellow
    $cacheDir = Join-Path $env:LOCALAPPDATA "KeePass\PluginCache"
    if (Test-Path $cacheDir) { Remove-Item $cacheDir -Recurse -Force -ErrorAction SilentlyContinue }
    Get-ChildItem $pluginsDir -Filter "KeeAnywhere*" -File -ErrorAction SilentlyContinue | Remove-Item -Force
}

Copy-Item $plgx $pluginsDir -Force
Write-Host "Deployed to $pluginsDir" -ForegroundColor Green

# 5. Run KeePass with --debug so PlgxPlugin.SaveCompilerResults writes the
#    compile log to %TEMP%\tmpXXXX.tmp on failure.
$cutoff = Get-Date
Write-Host ""
Write-Host "Launching KeePass --debug. Close it (and dismiss any error dialogs) to continue." -ForegroundColor Yellow
& $kpExe --debug | Out-Null
Get-Process KeePass -ErrorAction SilentlyContinue | Wait-Process -Timeout 5 -ErrorAction SilentlyContinue

# 6. Report.
$logs = Get-ChildItem $env:TEMP -File -ErrorAction SilentlyContinue |
    Where-Object { $_.LastWriteTime -gt $cutoff -and $_.Length -gt 0 -and $_.Extension -eq ".tmp" } |
    Sort-Object LastWriteTime -Descending

$compileLog = $null
foreach ($l in $logs) {
    $head = Get-Content $l.FullName -TotalCount 5 -ErrorAction SilentlyContinue
    if ($head -match "Compiler|csc\.exe|error CS") {
        $compileLog = $l
        break
    }
}

Write-Host ""
if ($compileLog) {
    Write-Host "=== plgx FAILED to compile ===" -ForegroundColor Red
    $errors = Get-Content $compileLog.FullName | Select-String "error CS\d" | Select-Object -ExpandProperty Line
    if ($errors) {
        Write-Host "Compiler errors:" -ForegroundColor Red
        $errors | Select-Object -First 20 | ForEach-Object { Write-Host "  $_" }
        if ($errors.Count -gt 20) { Write-Host "  ... ($($errors.Count - 20) more)" }
    }
    Write-Host ""
    Write-Host "Full log: $($compileLog.FullName)" -ForegroundColor DarkGray
    exit 1
}

$compiled = Get-ChildItem (Join-Path $env:LOCALAPPDATA "KeePass\PluginCache") -Filter "KeeAnywhere.dll" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
if ($compiled) {
    Write-Host "=== plgx compiled OK ===" -ForegroundColor Green
    Write-Host "Compiled assembly: $($compiled.FullName)" -ForegroundColor DarkGray
    Write-Host ""
    Write-Host "Note: a clean compile doesn't guarantee runtime load — KeePass's compatibility check"
    Write-Host "(PluginManager.CheckCompatibility) can still reject the assembly for transitive version"
    Write-Host "mismatches. If you saw a 'Could not load file or assembly' dialog, the plgx compiled"
    Write-Host "but failed the post-compile binding check."
    exit 0
}

Write-Host "=== inconclusive ===" -ForegroundColor Yellow
Write-Host "No compiler error log in TEMP and no compiled assembly in PluginCache."
Write-Host "KeePass may not have attempted to load the plgx (e.g. closed before reaching it)."
exit 2
