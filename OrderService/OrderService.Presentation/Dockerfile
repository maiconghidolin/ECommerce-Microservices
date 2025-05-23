FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY *.sln .
COPY ["OrderService.Application.Unit.Tests/OrderService.Application.Unit.Tests.csproj", "OrderService.Application.Unit.Tests/"]
COPY ["OrderService.Presentation.Integration.Tests/OrderService.Presentation.Integration.Tests.csproj", "OrderService.Presentation.Integration.Tests/"]
COPY ["OrderService.Application/OrderService.Application.csproj", "OrderService.Application/"]
COPY ["OrderService.Domain/OrderService.Domain.csproj", "OrderService.Domain/"]
COPY ["OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService.Infrastructure/"]
COPY ["OrderService.Presentation/OrderService.Presentation.csproj", "OrderService.Presentation/"]

RUN dotnet restore OrderService.sln

COPY . .

WORKDIR "/src/OrderService.Presentation"

RUN dotnet publish "./OrderService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081  
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Presentation.dll"]