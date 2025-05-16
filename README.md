# Finanzas Personales - Proyecto DAW2

Este proyecto contiene una API RESTful en **.NET 8** (`back`) y un frontend en **Vue 3 + Vite** (`front`), junto con una base de datos PostgreSQL.

## Clonar el repositorio
```bash
git clone https://github.com/cosmind-rusu/PROYECTODAW2.git
cd PROYECTODAW2
```

## Ejecutar en local

### Backend (.NET)
```bash
cd back
# Restaurar paquetes
dotnet restore
# Ejecutar
dotnet run
```
La API escuchará en http://localhost:5000 por defecto.

### Frontend (Vue)
```bash
cd front
npm install
npm run dev
```
El frontend estará disponible en http://localhost:5173 (puerto de Vite).

## Base de datos y migraciones

Asegúrate de tener PostgreSQL en ejecución o usa Docker (ver sección Docker).

1. (Opcional) Instalar la herramienta de EF Core CLI:
```bash
dotnet tool install --global dotnet-ef
```
2. Crear migraciones e inicializar la BD:
```bash
cd back
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Ejecutar con Docker Compose

Desde la raíz del proyecto:
```bash
docker-compose up --build
```

- API: http://localhost:5000
- Frontend: http://localhost:8080
- PostgreSQL: localhost:5432 (DB: `finanzasdb`, user: `postgres`, pass: `postgres`)

## Configuración de la conexión

En `back/appsettings.json` o mediante variable de entorno:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=db;Port=5432;Database=finanzasdb;Username=postgres;Password=postgres"
}
```

## Volúmenes Docker

- `db-data`: Persiste datos PostgreSQL en local.

---

¡Listo! Ahora puedes desarrollar y probar la aplicación en tu máquina o en contenedores Docker. Si tienes dudas, abre un issue en el repositorio.
