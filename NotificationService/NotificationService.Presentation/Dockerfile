FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY *.sln .
COPY ["NotificationService.Application.Unit.Tests/NotificationService.Application.Unit.Tests.csproj", "NotificationService.Application.Unit.Tests/"]
COPY ["NotificationService.Presentation.Integration.Tests/NotificationService.Presentation.Integration.Tests.csproj", "NotificationService.Presentation.Integration.Tests/"]
COPY ["NotificationService.Application/NotificationService.Application.csproj", "NotificationService.Application/"]
COPY ["NotificationService.Domain/NotificationService.Domain.csproj", "NotificationService.Domain/"]
COPY ["NotificationService.Infrastructure/NotificationService.Infrastructure.csproj", "NotificationService.Infrastructure/"]
COPY ["NotificationService.Presentation/NotificationService.Presentation.csproj", "NotificationService.Presentation/"]

RUN dotnet restore NotificationService.sln

COPY . .

WORKDIR "/src/NotificationService.Presentation"

RUN dotnet publish "./NotificationService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081  
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.Presentation.dll"]