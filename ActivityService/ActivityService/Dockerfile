# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiraj csproj fajl i restore dependencies
COPY ["ActivityService.csproj", "./"]
RUN dotnet restore "./ActivityService.csproj"

# Kopiraj ostatak koda i izgradi aplikaciju
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Ekspozicija porta
EXPOSE 80

ENTRYPOINT ["dotnet", "ActivityService.dll"]
