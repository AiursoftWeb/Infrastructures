FROM microsoft/aspnetcore-build AS build
WORKDIR /src
COPY . /src
WORKDIR /src/API
RUN dotnet restore
RUN dotnet publish --no-restore -c Release -o /app

FROM microsoft/aspnetcore
WORKDIR /app
ENV ASPNETCORE_URLS http://+:5001
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Aiursoft.API.dll"]
