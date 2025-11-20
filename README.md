# UniTask – Sistema de Gestión de Tareas Académicas

UniTask es una aplicación web diseñada para estudiantes que necesitan organizar materias, tareas, prioridades, etiquetas y recordatorios. Incluye autenticación con correo/contraseña, autenticación con Google y un sistema de impersonación para administradores.

Este documento describe cómo ejecutar tanto el backend (ASP.NET Core) como el frontend (React + Vite) por primera vez.

---

## Tecnologías utilizadas

### Backend
- ASP.NET Core (.NET 8)
- Entity Framework Core
- MySQL
- JWT Authentication
- Google OAuth

### Frontend
- React
- Vite
- Fetch API
- Google Identity Services

---

## Estructura del proyecto

Proyectofinalpow2/
├── Backend/
│ ├── Controllers/
│ ├── Data/
│ ├── Helpers/
│ ├── Models/
│ ├── appsettings.json
│ └── Proyectofinalpow2.csproj
└── Frontend/
├── src/
├── index.html
├── package.json
└── vite.config.js

yaml
Copiar código

---

## Requisitos previos

Antes de iniciar, instalá:

- .NET SDK 8
- Node.js 18 o superior
- MySQL Server 8

---

## Configuración de la base de datos MySQL

Ejecutar en MySQL:

```sql
CREATE DATABASE unitask CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
Asegurate de tener usuario y contraseña de MySQL correctamente configurados.

Backend – Ejecución inicial
1. Entrar a la carpeta del backend
bash
Copiar código
cd Backend
2. Configurar appsettings.json
Ejemplo mínimo:

json
Copiar código
{
  "ConnectionStrings": {
    "Default": "server=localhost;database=unitask;user=root;password=TU_PASSWORD;"
  },
  "Jwt": {
    "Key": "clave-super-secreta",
    "Issuer": "unitask-api",
    "Audience": "unitask-client"
  },
  "GoogleAuth": {
    "ClientIds": [
      "TU_CLIENT_ID.apps.googleusercontent.com"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
Modificar:

TU_PASSWORD por tu contraseña real de MySQL.

TU_CLIENT_ID por tu Client ID de Google.

3. Crear tablas (si usas migraciones)
bash
Copiar código
dotnet ef database update
Si no tenés migraciones, las tablas se crean automáticamente al ejecutar el backend.

4. Ejecutar el backend
bash
Copiar código
dotnet run
El servidor quedará escuchando en:

http://localhost:5000

https://localhost:5001

Frontend – Ejecución inicial
1. Entrar a la carpeta del frontend
bash
Copiar código
cd Frontend
2. Instalar dependencias
bash
Copiar código
npm install
3. Configurar la URL del backend
Asegurate de que en el frontend exista algo así:

js
Copiar código
const API_BASE = "http://localhost:5000";
Modificar si el backend usa otro puerto.

4. Configurar Google Login
En el componente GoogleButton:

js
Copiar código
client_id: "TU_CLIENT_ID.apps.googleusercontent.com"
5. Ejecutar el frontend
bash
Copiar código
npm run dev
Abrir el navegador en:

arduino
Copiar código
http://localhost:5173/
Conexión entre frontend y backend
Debe cumplirse:

Backend activo en http://localhost:5000

Frontend activo en http://localhost:5173

API_BASE configurado correctamente en el frontend

Si se usan cookies HttpOnly, agregar:

js
Copiar código
credentials: "include"
en las peticiones fetch.

Configurar CORS (si es necesario)
Si el frontend no puede comunicarse con el backend, agregar esto en Program.cs del backend:

csharp
Copiar código
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors();
Certificado HTTPS de desarrollo
Si aparece el mensaje:

pgsql
Copiar código
The ASP.NET Core developer certificate is not trusted.
Ejecutar:

bash
Copiar código
dotnet dev-certs https --trust
Probando la API
Login local
bash
Copiar código
POST http://localhost:5000/auth/login
Body:

json
Copiar código
{
  "email": "usuario@ejemplo.com",
  "password": "123456"
}
Listar materias del usuario autenticado
bash
Copiar código
GET http://localhost:5000/materias/mias
Crear tarea
bash
Copiar código
POST http://localhost:5000/tareas
Body:

json
Copiar código
{
  "materia_id": "GUID_DE_MATERIA",
  "titulo": "Informe final",
  "descripcion": "Entrega de la semana",
  "vence_en": "2025-11-20T23:59:00",
  "prioridad": "Alta"
}
Resumen rápido
Crear la base de datos unitask.

Configurar appsettings.json.

Ejecutar backend:

arduino
Copiar código
dotnet run
Ejecutar frontend:

arduino
Copiar código
npm install
npm run dev
Abrir:

arduino
Copiar código
http://localhost:5173/
Notas finales
El proyecto está listo para servir como sistema académico completo con:

Autenticación local y Google

Gestión de materias, tareas y etiquetas

Recordatorios con notificaciones

Impersonación para administración

Arquitectura clara con ASP.NET Core y React

Se puede extender fácilmente con Docker, deploy en VPS, Nginx, CI/CD o versiones móviles.
