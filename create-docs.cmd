
set pandoc=pandoc.exe

cd bin\x86\Debug\net6.0-windows\docs
rm *.html *.md

cd ..\..\..\..\Release\x86\net6.0-windows\docs
rm *.html *.md


cd ..\..\..\..\..\docs
%pandoc% -f markdown -o KeizerForClubs.FAQ.EN.html KeizerForClubs.FAQ.EN.md
%pandoc% -f markdown -o KeizerForClubs.FAQ.DE.html KeizerForClubs.FAQ.DE.md
%pandoc% -f markdown -o KeizerForClubs.FAQ.FR.html KeizerForClubs.FAQ.FR.md

%pandoc% -f markdown -o KeizerForClubs.EN.html KeizerForClubs.EN.md
%pandoc% -f markdown -o KeizerForClubs.FR.html KeizerForClubs.FR.md
%pandoc% -f markdown -o KeizerForClubs.DE.html KeizerForClubs.DE.md

cd ..

