# Dockerfile para el backend (.NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia y restaura dependencias
COPY ["*.csproj", "./"]
RUN dotnet restore

# Construye y publica
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "api.dll"]
