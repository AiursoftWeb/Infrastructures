echo start building...

cd Pylon
dotnet build
cd ..

cd API
dotnet publish --no-restore -c Release
cd ..

cd Developer
dotnet publish --no-restore -c Release
cd ..

cd OSS
dotnet publish --no-restore -c Release
cd ..

cd Account
dotnet publish --no-restore -c Release
cd ..

cd Stargate
dotnet publish --no-restore -c Release
cd ..

cd Wiki
dotnet publish --no-restore -c Release
cd ..

cd EE
dotnet publish --no-restore -c Release
cd ..

cd Colossus
dotnet publish --no-restore -c Release
cd ..

cd WWW
dotnet publish --no-restore -c Release
cd ..

exit