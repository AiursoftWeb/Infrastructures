echo start building...

dotnet restore

cd SDK
dotnet build
cd ..

cd Pylon
dotnet build
cd ..

cd Gateway
dotnet publish --no-restore -c Release
cd ..

cd Archon
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
