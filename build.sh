echo start building...
dotnet restore
find . -name "*.csproj"  -exec bash -c "dotnet publish '{}' -c Release --no-restore" \;
exit
