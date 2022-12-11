
set pandoc=pandoc.exe

cd bin\Debug\net6.0-windows\docs
rm *.html *.md


cd ..\..\..\..\docs
%pandoc% -f markdown -o KeizerForClubs.FAQ.EN.html KeizerForClubs.FAQ.EN.md
%pandoc% -f markdown -o KeizerForClubs.FAQ.DE.html KeizerForClubs.FAQ.DE.md
cd ..