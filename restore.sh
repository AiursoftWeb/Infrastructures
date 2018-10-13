echo start restoring...

cd Pylon
dotnet restore
cd ..

cd API
dotnet restore
cd ..

cd Developer
dotnet restore
cd ..

cd OSS
dotnet restore
cd ..

cd Account
dotnet restore
cd ..

cd Stargate
dotnet restore
cd ..

cd Wiki
dotnet restore
cd ..

cd EE
dotnet restore
cd ..

cd Colossus
dotnet restore
cd ..

cd WWW
dotnet restore
cd ..

exit