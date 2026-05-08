FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG MIRKO_NUGET_USERNAME
ARG MIRKO_NUGET_PWD

WORKDIR /src

# NuGet
COPY NuGet.config .

# Copio i csproj
COPY VisitService.API/VisitService.API.csproj VisitService.API/
COPY VisitService.Business/VisitService.Business.csproj VisitService.Business/
COPY VisitService.Repository/VisitService.Repository.csproj VisitService.Repository/
COPY VisitService.Shared/VisitService.Shared.csproj VisitService.Shared/

# Restore
RUN dotnet restore VisitService.API/VisitService.API.csproj

# Copio tutto il resto
COPY . .

WORKDIR /src/VisitService.API

RUN dotnet publish VisitService.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "VisitService.API.dll"]
