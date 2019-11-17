@echo off
setlocal


set /p string=<config/config.json

set version=%1
rem Remove quotes
set string=%string:"=%
rem Remove braces
set "string=%string:~2,-2%"
rem Change colon+space by equal-sign
set "string=%string:: ==%"
rem Separate parts at comma into individual assignments
set "%string:, =" & set "%"               

cd BackendApi
echo %cd%
start "Backend" dotnet BackendApi.dll --configuration "%build_mode%" --launch-profile "%launch_profile%"
                                                        
cd ../Frontend
start "Frontend" dotnet Frontend.dll --configuration "%build_mode%" --launch-profile "%launch_profile%"

cd ../TextListener
start "TextListener" dotnet TextListener.dll --configuration "%build_mode%" --launch-profile "%launch_profile%"

cd ../TextRankCalc
start "TextRankCalc" dotnet TextRankCalc.dll --configuration "%build_mode%" --launch-profile "%launch_profile%"

cd ../VowelConsCounter
for /l %%a in (1, 1, %counter_instances%+0) do (
    start "VowelConsCounter" dotnet VowelConsCounter.dll --configuration "%build_mode%" --launch-profile "%launch_profile%"
)

cd ../VowelConsRater
for /l %%a in (1, 1, %rater_instances%+0) do (
    start "VowelConsRater" dotnet VowelConsRater.dll --configuration "%build_mode%" --launch-profile "%launch_profile%"
)

cd ../TextStatistics
start "TextStatistics" dotnet TextStatistics.dll --configuration "%build_mode%" --launch-profile "%launch_profile%"

cd ../TextProcessingLimiter
start "TextProcessingLimiter" dotnet TextProcessingLimiter.dll "%processing_limit%" --configuration "%build_mode%" --launch-profile "%launch_profile%"