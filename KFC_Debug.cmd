@echo off

@echo off
set FusionLogPath=%cd%
set COMPLUS_ForceLog=1
set COMPLUS_LogLevel=3
set COMPLUS_LogFailures=1
set COMPLUS_LogBind=1
set COMPLUS_LogPath=%FusionLogPath%
start "" ".\KFC2.exe" -d -fx
rem start "" ".\KeizerForClubs.exe" "-d"

