@echo off
set version=2.1.0
set zip="packages\7-Zip.CommandLine.18.1.0\tools\7za.exe"
set msbuildcmd="C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsMSBuildCmd.bat"

if not exist %msbuildcmd% goto error
call %msbuildcmd%

:cleanup
if not exist build mkdir build
del /s /f /q build\*

if not exist build\dist mkdir build\dist

:build
nuget.exe restore
msbuild KeeAnywhere.sln /p:Configuration=Release /t:Clean,Build /fl /flp:logfile=build\build.log
if %errorlevel% NEQ 0 goto error

:package_plgx
copy KeeAnywhere\bin\Release\KeeAnywhere.plgx build\dist\KeeAnywhere-%version%.plgx

:package_zip
cd KeeAnywhere\bin\Release
..\..\..\%zip% a -tzip ..\..\..\build\dist\KeeAnywhere-%version%.zip -x!*.plgx -x!*.pdb -x!*.xml -x!*.config -x!KeePass.*
cd ..\..\..
if %errorlevel% NEQ 0 goto error

:package_chocolatey
xcopy /s /i chocolatey build\chocolatey
xcopy KeeAnywhere\bin\Release\KeeAnywhere.plgx build\chocolatey\tools
choco pack build\chocolatey\keepass-plugin-keeanywhere.nuspec --version %version% --outputdirectory build\dist  
if %errorlevel% NEQ 0 goto error

:final
goto end

:error
echo !
echo !
echo !
echo Fehler im Buildvorgang
echo !
echo !
echo !


:end
pause
