FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY ./AdminApplication/*.csproj ./AdminApplication/

RUN dotnet restore

COPY . .

WORKDIR /src/AdminApplication
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 44315
EXPOSE 5001
EXPOSE 5000

ENTRYPOINT ["dotnet", "AdminApplication.dll"]