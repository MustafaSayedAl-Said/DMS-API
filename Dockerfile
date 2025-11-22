# ====== Base image ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ====== Base image ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


# ====== Build image ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["DMS.API/DMS.API.csproj", "DMS.API/"]
COPY ["DMS.Core/DMS.Core.csproj", "DMS.Core/"]
COPY ["DMS.Infrastructure/DMS.Infrastructure.csproj", "DMS.Infrastructure/"]
COPY ["DMS.Services/DMS.Services.csproj", "DMS.Services/"]

RUN dotnet restore "DMS.API/DMS.API.csproj"

COPY . .

WORKDIR "/src/DMS.API"
RUN dotnet build -c Release -o /app/build

# ====== Publish ======
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ====== Final Runtime ======
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/wwwroot/documents

# Railway provides PORT env variable
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}

ENTRYPOINT ["dotnet", "DMS.API.dll"]
