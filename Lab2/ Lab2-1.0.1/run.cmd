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
                                                             
cd Backend
start "Backend" dotnet Backend.dll --configuration "%build_mode%" --launch-profile "%launch_profile%"
                                                        
cd ../Frontend
start "Frontend" dotnet Frontend.dll --configuration "%build_mode%" --launch-profile "%launch_profile%"