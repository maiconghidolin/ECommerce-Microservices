FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY *.sln .
COPY ["CatalogService.Application.Tests/CatalogService.Application.Unit.Tests.csproj", "CatalogService.Application.Tests/"]
COPY ["CatalogService.Presentation.Integration.Tests/CatalogService.Presentation.Integration.Tests.csproj", "CatalogService.Presentation.Integration.Tests/"]
COPY ["CatalogService.Application/CatalogService.Application.csproj", "CatalogService.Application/"]
COPY ["CatalogService.Domain/CatalogService.Domain.csproj", "CatalogService.Domain/"]
COPY ["CatalogService.Infrastructure/CatalogService.Infrastructure.csproj", "CatalogService.Infrastructure/"]
COPY ["CatalogService.Presentation/CatalogService.Presentation.csproj", "CatalogService.Presentation/"]

RUN dotnet restore CatalogService.sln

COPY . .

WORKDIR "/src/CatalogService.Presentation"

RUN dotnet publish "./CatalogService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081  
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.Presentation.dll"]