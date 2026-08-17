# ============ ETAPA DE DESARROLLO ============
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS development

# Instalar dotnet-watch
RUN dotnet tool install -g dotnet-watch

# Agregar herramientas al PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Exponer puertos
EXPOSE 8080
EXPOSE 8081

WORKDIR /app

# Copiar archivos de proyecto primero (para mejor caching)
COPY ["Api/Api.csproj", "Api/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "Api/Api.csproj"

# Copiar el resto del código
COPY . .

# Variables de entorno para desarrollo
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER=true

# Comando para desarrollo con hot reload
# NOTA: El proyecto está en /app/Api/Api.csproj
CMD ["dotnet", "watch", "run", "--urls=http://0.0.0.0:8080", "--project", "Api/Api.csproj"]

# ============ ETAPA DE CONSTRUCCIÓN ============
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto
COPY ["Api/Api.csproj", "Api/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "Api/Api.csproj"

# Copiar el resto del código
COPY . .

# Publicar la aplicación
WORKDIR "/src/Api"
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ============ ETAPA DE PRODUCCIÓN ============
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS production
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copiar archivos publicados
COPY --from=build /app/publish .

# Configurar variables de entorno para producción
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Comando para producción
ENTRYPOINT ["dotnet", "Api.dll"]