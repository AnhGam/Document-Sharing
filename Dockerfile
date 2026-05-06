# Dockerfile cho API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["document-sharing-manager-api/document-sharing-manager-api.csproj", "document-sharing-manager-api/"]
COPY ["document-sharing-manager.Core/document-sharing-manager.Core.csproj", "document-sharing-manager.Core/"]
COPY ["document-sharing-manager.Infrastructure/document-sharing-manager.Infrastructure.csproj", "document-sharing-manager.Infrastructure/"]
RUN dotnet restore "document-sharing-manager-api/document-sharing-manager-api.csproj"
COPY . .
WORKDIR "/src/document-sharing-manager-api"
RUN dotnet build "document-sharing-manager-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "document-sharing-manager-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENTRYPOINT ["dotnet", "document-sharing-manager-api.dll"]
