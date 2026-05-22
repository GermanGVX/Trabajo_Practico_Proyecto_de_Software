# Trabajo Práctico: Proyecto de Software

Este repositorio contiene el código fuente del Trabajo Práctico de la materia. El sistema es una API desarrollada con **.NET 8** (C#) siguiendo una Arquitectura en Capas (Application, Domain, Infrastructure), utilizando **Entity Framework Core** para el acceso a datos e incluye una interfaz gráfica en HTML/CSS/JS.

## Requisitos Previos

Antes de comenzar, asegúrate de tener instalado lo siguiente en tu entorno de desarrollo:

* [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (o superior).
* [SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (Express o Developer edition).
* [Visual Studio 2022](https://visualstudio.microsoft.com/) (recomendado) o Visual Studio Code.
* *Opcional:* SQL Server Management Studio (SSMS) o Azure Data Studio para visualizar la base de datos.

---
## Estructura del Proyecto
El proyecto está dividido en dos partes independientes:
- **Backend (API)**: Carpeta `Trabajo_Practoco_Proyecto_de_Software/` - API REST con .NET 8
- **Frontend (Client)**: Carpeta `Client/` - Interfaz HTML/CSS/JS vanilla

## Pasos para levantar el proyecto

### 1. Clonar el repositorio

Abre tu terminal (Git Bash, PowerShell o CMD) y ejecuta los siguientes comandos para descargar el código y entrar a la carpeta del proyecto:
git clone https://github.com/GermanGVX/Trabajo_Practico_Proyecto_de_Software.git
cd Trabajo_Practico_Proyecto_de_software

### 2. Comprobar paquetes

*Microsoft.EntityFrameworkCore.Design (9.0.15)

*Microsoft.Extensions.Hosting.Abstractions (9.0.15)

*Swashbuckle.AspNetCore (6.6.2)


### 3. Conexion a db

*configurar el appsetting.json

"ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TP_Proyecto_de_Software;Trusted_Connection=True;TrustServerCertificate=True;"
}



##  Configuración del Frontend

La URL de la API está parametrizada en `Client/js/config.js`:
```javascript 
window.API_BASE_URL = "https://localhost:7129/api";
```
*Abre tu terminal (consola de administrador de paquetes) y ejecuta los siguientes comandos para conectar a la base de datos:
Update-Database


##  Ejecución

### Backend (API)
```bash
cd Trabajo_Practoco_Proyecto_de_Software
dotnet run
```
### Frontend (Client)

OPCIÓN A:
- Ejecutá el backend y navegá a `https://localhost:7129/auth.html`
- El backend sirve los archivos estáticos desde la carpeta `Client/`

OPCIÓN B:
- Ejecutá el backend
- En Visual Studio Code en la carpeta de Client dale click derecho a auth.html y Open With Live Server

### CORS
El backend está configurado para aceptar peticiones desde:
-`https://localhost:7129` (mismo origen)
-`http://localhost:5500` (Live Server)
-`http://127.0.0.1:5500` (Live Server - IP directa)
-`null` (archivo local)

## SEED
El sistema incluye seed de datos iniciales:
- **Evento**: Concierto de Rock
- **Sectores**: Campo ($5000) y Platea ($8000)
- **Butacas**: 50 por sector

## URLs
Swagger: https://localhost:7129/swagger
Frontend: https://localhost:7129/auth.html

---
