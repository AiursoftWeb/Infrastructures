FROM microsoft/aspnetcore-build AS build
WORKDIR /src
COPY . /src
WORKDIR /src/Wiki
RUN dotnet restore
RUN dotnet publish --no-restore -c Release -o /app

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Aiursoft.Wiki.dll"]
