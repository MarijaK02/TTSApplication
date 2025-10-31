FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY ./TTS.Web/*.csproj ./TTS.Web/
COPY ./TTS.Domain/*.csproj ./TTS.Domain/
COPY ./TTS.Repository/*.csproj ./TTS.Repository/
COPY ./TTS.Service/*.csproj ./TTS.Service/

RUN dotnet restore
COPY . .
WORKDIR /src/TTS.Web
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "TTS.Web.dll"]
