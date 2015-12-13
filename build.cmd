@echo off
set version=0.2.0-unstable
set zip="packages\7-Zip.CommandLine.9.20.0\tools\7za.exe"

call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86

:cleanup
if not exist build mkdir build
del /s /f /q build\*

if not exist build\bin mkdir build\bin
if not exist build\dist mkdir build\dist

:build
msbuild KeeAnywhere.sln /p:Configuration=Release /t:Clean,Build /fl /flp:logfile=build\build.log
if %errorlevel% NEQ 0 goto error

:package
copy KeeAnywhere\bin\Release\KeeAnywhere.plgx build\dist\KeeAnywhere-%version%.plgx

xcopy KeeAnywhere\bin\Release\*.* build\bin
del build\bin\*.plgx build\bin\*.pdb build\bin\*.xml
%zip% a -tzip build\dist\KeeAnywhere-%version%.zip .\build\bin\*



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
