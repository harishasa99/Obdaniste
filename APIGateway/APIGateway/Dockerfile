# Faza 1: Baza za runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 5000

# Faza 2: SDK za build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopiraj .csproj fajl
COPY ["APIGateway.csproj", "./"]

# Restore nuget paketa
RUN dotnet restore "./APIGateway.csproj"

# Kopiraj ostatak koda
COPY . .

WORKDIR "/src"

# Build aplikacije
RUN dotnet build "./APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Faza 3: Publish aplikacije
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Faza 4: Finalna slika za runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.dll"]
