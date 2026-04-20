FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY SBSC_Store/SBSC_Store.csproj SBSC_Store/
COPY Contracts/Contracts.csproj Contracts/
COPY Entities/Entities.csproj Entities/
COPY Shared/Shared.csproj Shared/
COPY LoggerService/LoggerService.csproj LoggerService/
COPY Presentation/Presentation.csproj Presentation/
COPY Service.Contracts/Service.Contracts.csproj Service.Contracts/
COPY Repository/Repository.csproj Repository/
COPY Service/Service.csproj Service/
RUN dotnet restore SBSC_Store/SBSC_Store.csproj
COPY . .
WORKDIR "/src/SBSC_Store"
RUN dotnet build "./SBSC_Store.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SBSC_Store.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
RUN mkdir -p /app/uploads && chown -R $APP_UID /app/uploads
VOLUME ["/app/uploads"]
USER $APP_UID
ENTRYPOINT ["dotnet", "SBSC_Store.dll"]
