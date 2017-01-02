@echo off
set version=1.3.1
set zip="packages\7-Zip.CommandLine.9.20.0\tools\7za.exe"
set msbuildcmd="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat"

if not exist %msbuildcmd% goto error
call %msbuildcmd%

:cleanup
if not exist build mkdir build
del /s /f /q build\*

if not exist build\bin mkdir build\bin
if not exist build\dist mkdir build\dist

:build
nuget.exe restore
msbuild KeeAnywhere.sln /p:Configuration=Release /t:Clean,Build /fl /flp:logfile=build\build.log
if %errorlevel% NEQ 0 goto error

:package
copy KeeAnywhere\bin\Release\KeeAnywhere.plgx build\dist\KeeAnywhere-%version%.plgx

xcopy KeeAnywhere\bin\Release\*.* build\bin
del build\bin\*.plgx build\bin\*.pdb build\bin\*.xml build\bin\*.config build\bin\KeePass.*
%zip% a -tzip build\dist\KeeAnywhere-%version%.zip .\build\bin\*

xcopy /s /i chocolatey build\chocolatey
xcopy KeeAnywhere\bin\Release\KeeAnywhere.plgx build\chocolatey\tools
choco pack build\chocolatey\keepass-plugin-keeanywhere.nuspec --version %version% --outputdirectory build\dist  


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
