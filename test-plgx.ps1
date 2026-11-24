<#
.SYNOPSIS
    Builds KeeAnywhere.plgx, deploys it to a portable KeePass and checks whether KeePass compiles it.

.DESCRIPTION
    By default the KeePass in lib\ is used - the same version KeeAnywhere is built against.
    It is run in portable mode (lib\KeePass.config.xml), so the configuration of your
    productive KeePass installation is not touched.

    Note: KeeAnywhere accounts are stored per Windows user (Credential Manager or local
    secure store), not per KeePass installation - so test and productive use share them.

    Based on test-plgx.ps1 from PR #486 by Stephane (@Fulgan).

.PARAMETER KeePassDir
    Folder of a portable KeePass (containing KeePass.exe). Default: lib\ of this repository.
    Download: https://keepass.info/download.html (Portable ZIP)

.PARAMETER Configuration
    Build configuration. PLGX is only built for Release.

.PARAMETER Cleanup
    Wipe %LOCALAPPDATA%\KeePass\PluginCache and previously deployed KeeAnywhere files first.
    A stale cache entry can mask a real compile failure.
#>
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

# 1. Locate KeePass (default: lib\ of this repository).
if (-not $KeePassDir) { $KeePassDir = Join-Path $repoRoot "lib" }
if (-not (Test-Path (Join-Path $KeePassDir "KeePass.exe"))) {
    throw "KeePass.exe not found in '$KeePassDir'."
}

$kpExe = (Resolve-Path (Join-Path $KeePassDir "KeePass.exe")).Path
$pluginsDir = Join-Path $KeePassDir "Plugins"
if (-not (Test-Path $pluginsDir)) { New-Item -ItemType Directory -Path $pluginsDir | Out-Null }

# Portable mode: without a writable KeePass.config.xml next to KeePass.exe, KeePass would use
# the user configuration in %APPDATA%\KeePass - i.e. the one of your productive installation.
$kpConfig = Join-Path $KeePassDir "KeePass.config.xml"
if (-not (Test-Path $kpConfig)) {
    Write-Host "Creating $kpConfig (portable mode)." -ForegroundColor Yellow
    $xml = @(
        '<?xml version="1.0" encoding="utf-8"?>',
        '<Configuration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">',
        '	<Meta>',
        '		<PreferUserConfiguration>false</PreferUserConfiguration>',
        '	</Meta>',
        '</Configuration>'
    ) -join "`r`n"
    [System.IO.File]::WriteAllText($kpConfig, $xml, (New-Object System.Text.UTF8Encoding($true)))
}

$kpVersion = (Get-Item $kpExe).VersionInfo.FileVersion
Write-Host "Using KeePass $kpVersion from $KeePassDir" -ForegroundColor Cyan

# 2. Do not touch running KeePass instances (KeePass --exit-all would close ALL of them,
#    including a productive one with unsaved changes).
$running = Get-Process KeePass -ErrorAction SilentlyContinue
if ($running) {
    throw "KeePass is running (PID $($running.Id -join ', ')). Please close all KeePass instances and run again."
}

# 3. Build (Release triggers PlgxTool's AfterBuild target).
$vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vswhere)) { throw "vswhere.exe not found. Install Visual Studio 2017+ or VS Build Tools." }
$msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
if (-not $msbuild) { throw "MSBuild not found via vswhere." }

$sln = Join-Path $repoRoot "KeeAnywhere.sln"
Write-Host "Restoring packages..." -ForegroundColor Cyan
# Restore NuGet packages (packages.config) via MSBuild
& $msbuild $sln "-t:restore" "-p:RestorePackagesConfig=true" "/nologo" "/v:q" | Out-Null
if ($LASTEXITCODE -ne 0) { throw "Package restore failed." }

Write-Host "Building $Configuration..." -ForegroundColor Cyan
& $msbuild (Join-Path $repoRoot "KeeAnywhere\KeeAnywhere.csproj") "/p:Configuration=$Configuration" "/t:Clean,Build" "/nologo" "/v:q" | Out-Null
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

$plgx = Join-Path $repoRoot "KeeAnywhere\bin\$Configuration\KeeAnywhere.plgx"
if (-not (Test-Path $plgx)) { throw "$plgx wasn't produced. PlgxTool only runs for Release." }
Write-Host "Built: $plgx" -ForegroundColor Green

# 4. Optional cache reset.
$cacheDir = Join-Path $env:LOCALAPPDATA "KeePass\PluginCache"
if ($Cleanup) {
    Write-Host "Cleanup: wiping PluginCache and prior KeeAnywhere deployment." -ForegroundColor Yellow
    if (Test-Path $cacheDir) { Remove-Item $cacheDir -Recurse -Force -ErrorAction SilentlyContinue }
    Get-ChildItem $pluginsDir -Filter "KeeAnywhere*" -File -ErrorAction SilentlyContinue | Remove-Item -Force
}

Copy-Item $plgx $pluginsDir -Force
Write-Host "Deployed to $pluginsDir" -ForegroundColor Green

# 5. Run KeePass with --debug, so KeePass writes the compiler output to %TEMP% on failure.
$cutoff = Get-Date
Write-Host ""
Write-Host "Launching KeePass --debug. Test the plugin, then close KeePass (and dismiss any error dialogs) to continue." -ForegroundColor Yellow
Start-Process -FilePath $kpExe -ArgumentList "--debug" -Wait

# 6. Report.
# Heuristic: look for compiler output files KeePass wrote to %TEMP% since launch.
$compileLog = Get-ChildItem $env:TEMP -File -ErrorAction SilentlyContinue |
    Where-Object { $_.LastWriteTime -gt $cutoff -and $_.Length -gt 0 -and $_.Extension -eq ".tmp" } |
    Sort-Object LastWriteTime -Descending |
    Where-Object { Select-String -Path $_.FullName -Pattern "error CS\d+" -Quiet -ErrorAction SilentlyContinue } |
    Select-Object -First 1

Write-Host ""
if ($compileLog) {
    Write-Host "=== PLGX FAILED to compile ===" -ForegroundColor Red
    $errors = @(Select-String -Path $compileLog.FullName -Pattern "error CS\d+" | Select-Object -ExpandProperty Line)
    $errors | Select-Object -First 20 | ForEach-Object { Write-Host "  $_" }
    if ($errors.Count -gt 20) { Write-Host "  ... ($($errors.Count - 20) more)" }
    Write-Host ""
    Write-Host "Full log: $($compileLog.FullName)" -ForegroundColor DarkGray
    exit 1
}

$compiled = Get-ChildItem $cacheDir -Filter "KeeAnywhere.dll" -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.LastWriteTime -gt $cutoff } |
    Select-Object -First 1
if ($compiled) {
    Write-Host "=== PLGX compiled OK ===" -ForegroundColor Green
    Write-Host "Compiled assembly: $($compiled.FullName)" -ForegroundColor DarkGray
    Write-Host ""
    Write-Host "Note: a clean compile doesn't guarantee a working plugin. If KeePass showed a"
    Write-Host "'Could not load file or assembly' dialog, there is still a dependency version mismatch."
    exit 0
}

Write-Host "=== inconclusive ===" -ForegroundColor Yellow
Write-Host "No compiler error found in %TEMP% and no freshly compiled KeeAnywhere.dll in PluginCache."
Write-Host "Tip: run again with -Cleanup - KeePass reuses a cached compile of an unchanged PLGX."
exit 2
