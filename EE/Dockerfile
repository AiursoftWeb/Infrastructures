FROM microsoft/aspnetcore-build AS build
WORKDIR /src
COPY . /src
WORKDIR /src/EE
RUN dotnet restore
RUN dotnet publish --no-restore -c Release -o /app

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Aiursoft.EE.dll"]
