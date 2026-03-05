# MOVEO_BACK

![.NET CI/CD Pipeline](https://github.com/USER/MOVEO_BACK/actions/workflows/dotnet-ci.yml/badge.svg)

Backend API para el sistema de enrutamiento logístico y últimas millas MOVEO.

## 🧪 Estrategia de Pruebas

Este proyecto cuenta con una robusta suite de pruebas automatizadas protegida por Integración Continua (CI) mediante GitHub Actions:

- **Pruebas de Humo (Smoke Tests):** Verificación Fail-Fast en segundos de los 5 flujos más críticos (Auth y Rutas). Si fallan, el pipeline se aborta.
- **Tests Unitarios:** 87 pruebas de caja negra aislando la capa `Moveo.Negocio` empleando `Moq`.
- **Tests de Integración:** 12 flujos completos levantando la API en memoria con `WebApplicationFactory` interactuando contra una base de datos temporal `SQLite`.

##🚀 Entornos

- **Framework:** .NET 8
- **Base de Datos (Producción):** PostgreSQL
- **Base de Datos (Tests):** SQLite In-Memory
