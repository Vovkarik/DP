@echo off
setlocal

set /p string=<src/config/config.json

set version=%1
rem Remove quotes
set string=%string:"=%
rem Remove braces
set "string=%string:~2,-2%"
rem Change colon+space by equal-sign
set "string=%string:: ==%"
rem Separate parts at comma into individual assignments
set "%string:, =" & set "%"                 
SET _buildPath= "%project_name%-%version%"

mkdir "%_buildPath%"
cd ../"%_buildPath%"

cd src/BackendApi
dotnet publish --configuration "%build_mode%" -f netcoreapp2.2 -o "../../%_buildPath%/BackendApi" /property:PublishWithAspNetCoreTargetManifest=false

cd ../Frontend
dotnet publish --configuration "%build_mode%" -f netcoreapp2.2 -o "../../%_buildPath%/Frontend" /property:PublishWithAspNetCoreTargetManifest=false

cd ../TextListener
dotnet publish --configuration "%build_mode%" -f netcoreapp2.2 -o "../../%_buildPath%/TextListener" /property:PublishWithAspNetCoreTargetManifest=false

cd ../
cd ../
echo %cd%
copy run.cmd "%_buildPath%"
copy stop.cmd "%_buildPath%"