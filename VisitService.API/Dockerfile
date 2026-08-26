# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# =========================
# ARG per GitHub Packages
# =========================
ARG MIRKO_NUGET_USERNAME
ARG MIRKO_NUGET_PWD

# =========================
# REGISTRA FEED NUGET PRIVATO
# =========================
RUN dotnet nuget add source https://nuget.pkg.github.com/Mirko627/index.json \
    --name github \
    --username $MIRKO_NUGET_USERNAME \
    --password $MIRKO_NUGET_PWD \
    --store-password-in-clear-text

# =========================
# COPY csproj (layer caching)
# =========================
COPY NuGet.config . 
COPY VisitService.API/VisitService.API.csproj VisitService.API/
COPY VisitService.Business/VisitService.Business.csproj VisitService.Business/
COPY VisitService.Repository/VisitService.Repository.csproj VisitService.Repository/
COPY VisitService.Shared/VisitService.Shared.csproj VisitService.Shared/

# =========================
# RESTORE
# =========================
RUN dotnet restore VisitService.API/VisitService.API.csproj

# =========================
# COPY FULL SOURCE
# =========================
COPY . .

WORKDIR /src/VisitService.API

# =========================
# PUBLISH
# =========================
RUN dotnet publish VisitService.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "VisitService.API.dll"]