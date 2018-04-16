echo start building...

cd Pylon
dotnet build
cd ..

cd API
dotnet publish --no-restore
cd ..

cd Developer
dotnet publish --no-restore
cd ..

cd OSS
dotnet publish --no-restore
cd ..

cd Account
dotnet publish --no-restore
cd ..

cd Stargate
dotnet publish --no-restore
cd ..

cd Kahla.Server
dotnet publish --no-restore
cd ..

cd Kahla.Home
dotnet publish --no-restore
cd ..

cd Wiki
dotnet publish --no-restore
cd ..

cd EE
dotnet publish --no-restore
cd ..

cd Colossus
dotnet publish --no-restore
cd ..

exit