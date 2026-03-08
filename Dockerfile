# ETAPA 1: Compilación (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiamos los archivos de proyecto (.csproj) para restaurar dependencias
# Hacemos esto primero para aprovechar la caché de Docker y que sea más rápido
COPY ["Moveo.API/Moveo.API.csproj", "Moveo.API/"]
COPY ["Moveo.Negocio/Moveo.Negocio.csproj", "Moveo.Negocio/"]
COPY ["Moveo.Modelos/Moveo.Modelos.csproj", "Moveo.Modelos/"]
COPY ["Moveo.AccesoDatos/Moveo.AccesoDatos.csproj", "Moveo.AccesoDatos/"]

# 2. Restauramos las piezas (incluyendo el AWSSDK de SNS que instalamos)
RUN dotnet restore "Moveo.API/Moveo.API.csproj"

# 3. Copiamos el resto del código de todos los proyectos
COPY . .

# 4. Compilamos el proyecto principal (Moveo.API)
WORKDIR "/src/Moveo.API"
RUN dotnet build "Moveo.API.csproj" -c Release -o /app/build

# ETAPA 2: Publicación (Publish)
FROM build AS publish
RUN dotnet publish "Moveo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ETAPA 3: Imagen Final (Runtime)
# Usamos una imagen mucho más pequeña que solo tiene lo necesario para ejecutar .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

# Configuramos el puerto (5000 es el que hemos estado usando)
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# El comando que arranca tu API
ENTRYPOINT ["dotnet", "Moveo.API.dll"]