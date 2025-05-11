FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ECommerceApi.csproj", "./"]
RUN dotnet restore "./ECommerceApi.csproj"
COPY . . 
RUN dotnet build "ECommerceApi.csproj" -c Release -o /app/build

# Install EF Core tools
RUN dotnet tool restore

FROM build AS publish
RUN dotnet publish "ECommerceApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY entrypoint.sh . 

# Pastikan skrip bisa dieksekusi
RUN chmod +x ./entrypoint.sh

# Gunakan entrypoint.sh sebagai entry point
ENTRYPOINT ["./entrypoint.sh"]
