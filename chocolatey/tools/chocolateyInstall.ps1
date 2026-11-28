# powershell v2 compatibility
$psVer = $PSVersionTable.PSVersion.Major
if ($psver -ge 3) {
  function Get-ChildItemDir {Get-ChildItem -Directory $args}
} else {
  function Get-ChildItemDir {Get-ChildItem $args}
}

$packageName = 'keepass-plugin-keeanywhere'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$packageSearch = 'KeePass Password Safe'

Write-Verbose "Searching registry for installed KeePass..."
$regPath = Get-ItemProperty -Path @('HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*',
                                    'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*',
                                    'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*') `
                            -ErrorAction:SilentlyContinue `
           | Where-Object {$_.DisplayName -like "$packageSearch*" `
                           -and `
                           $_.DisplayVersion -ge 2.0 `
                           -and `
                           $_.DisplayVersion -lt 3.0 } `
           | ForEach-Object {$_.InstallLocation}
$installPath = $regPath
if (! $installPath) {
  Write-Verbose "Searching tools location for portable install..."
  $binRoot = Get-ToolsLocation
  $portPath = Join-Path $binRoot "keepass"
  $installPath = Get-ChildItemDir $portPath* -ErrorAction SilentlyContinue
}
if (! $installPath) {
  Write-Verbose "Searching $env:Path for unregistered install..."
  $installFullName = (Get-Command keepass -ErrorAction SilentlyContinue).Path
  if ($installFullName) {
    $installPath = [io.path]::GetDirectoryName($installFullName)
  }
}
if (! $installPath) {
  Write-Warning "$($packageSearch) not found."
  throw
}
Write-Verbose "`t...found."

Write-Verbose "Searching for plugin directory..."
$pluginPath = (Get-ChildItemDir $installPath\Plugin*).FullName
if ($pluginPath.Count -eq 0) {
  $pluginPath = Join-Path $installPath "Plugins"
  [System.IO.Directory]::CreateDirectory($pluginPath)
}
# The DLLs of a loaded plugin are locked, so KeePass must be closed
if (Get-Process -Name "KeePass" -ErrorAction SilentlyContinue) {
  Write-Warning "$($packageSearch) is running. Please save any opened databases and close $($packageSearch) before installing or updating KeeAnywhere."
  throw
}

# Versions up to 2.3.0 were installed as PLGX file - remove it, otherwise KeeAnywhere is loaded twice
Remove-Item -Path (Join-Path $pluginPath "$($packageName).plgx") -Force -ErrorAction SilentlyContinue

# DLL version in its own subfolder; clear it first, so no libraries of an older version are left
$installDir = Join-Path $pluginPath "KeeAnywhere"
Remove-Item -Path $installDir -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item -Path (Join-Path $toolsDir "KeeAnywhere") -Destination $installDir -Recurse

Write-Host "$($packageName) will be loaded the next time KeePass is started."
Write-Host "Please note this plugin may require additional configuration. Look for a new entry in KeePass' Menu>Tools"