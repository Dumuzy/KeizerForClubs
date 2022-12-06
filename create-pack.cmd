set vers=1.1.3
set temp0=C:\tmp\kfc-pack\
set packname=Keizer_%vers%
set tempdir=%temp0%%packname%


md %tempdir%
del /Q %tempdir%\*.*
del /Q %tempdir%\..\*.zip
del /Q %tempdir%\cfg
del /Q %tempdir%\docs
del /Q %tempdir%\export


cd bin\Debug\net6.0-windows
copy KFC2.exe  %tempdir%\KeizerForClubs.exe
xcopy *.dll %tempdir% /S
xcopy *.json  %tempdir% /S
xcopy  *.s3db  %tempdir% /S
xcopy   *.pdf %tempdir% /S
xcopy    *.txt %tempdir% /S
xcopy    *.css  %tempdir% /S
xcopy    *.xsl %tempdir% /S



cd %tempdir%\..

7za a -tzip kfc %packname% 
rename kfc.zip %packname%.zip

rd /S /Q %tempdir%\cfg
rd /S /Q %tempdir%\docs
rd /S /Q %tempdir%\export
del /Q %tempdir%\*.*
rd %tempdir%

echo off
echo "%packname%.zip created in %temp0%"
