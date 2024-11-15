rem
rem     This script packs together a Keizer_...zip package. 
rem     It uses the  executables from the bin\x86\Release-directory. 
rem     It takes the version number from KFC2.csproj. 
rem     To work, this script needs robocopy,  tclsh.exe and 7za.exe in PATH. 

@echo off
set "startdir=%cd%"
set projfile=KFC2.csproj
rem The following seems to be the way to go to put the result of a command into 
rem a variable in cmd-scripts. It is truly astonishing. 
for /f usebackq %%i in (`tclsh extract-vers.tcl %projfile%`) do (
  set vers=%%i
)

echo Version parsed from %projfile% is %vers%

set temp0=C:\tmp\kfc-pack\
set packname=Keizer_%vers%
set tempdir=%temp0%%packname%


md %tempdir%
del /Q %tempdir%\*.*
del /Q %tempdir%\..\*.zip
del /Q %tempdir%\cfg
del /Q %tempdir%\docs
del /Q %tempdir%\export


cd bin\x86\Release\net6.0-windows
copy KFC2.exe  %tempdir%\KeizerForClubs.exe
copy ..\..\..\..\cfg\* cfg
robocopy ..\..\..\..\docs\ docs *.html *.png *.pdf *.txt /E


robocopy .  %tempdir% *.dll *.json *.s3db Keizer*.html *.ini *.png *.pdf *.txt *.css *.xsl /E

cd %tempdir%\..

7za a -tzip kfc %packname% 
rename kfc.zip %packname%.zip

rd /S /Q %tempdir%\cfg
rd /S /Q %tempdir%\docs
rd /S /Q %tempdir%\export
del /Q %tempdir%\*.*
rd %tempdir%

echo %packname%.zip created in %temp0%
cd "%startdir%"
