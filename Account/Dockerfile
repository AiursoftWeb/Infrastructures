FROM microsoft/aspnetcore-build AS build
WORKDIR /src
COPY . /src
RUN dotnet restore
WORKDIR /src/Account
RUN dotnet publish --no-restore -c Release -o /app

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Aiursoft.Account.dll"]
