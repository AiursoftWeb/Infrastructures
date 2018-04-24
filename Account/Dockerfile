FROM microsoft/aspnetcore-build AS build
WORKDIR /src
COPY . /src
WORKDIR /src/Account
RUN dotnet restore
RUN dotnet publish --no-restore -c Release -o /app

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS http://+:5000
ENTRYPOINT ["dotnet", "Aiursoft.Account.dll"]
