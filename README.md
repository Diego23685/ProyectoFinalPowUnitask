# UniTask – Sistema de Gestión de Tareas Académicas

UniTask es una aplicación web diseñada para estudiantes que desean organizar materias, tareas, prioridades, etiquetas y recordatorios. Incluye autenticación con correo y contraseña, inicio de sesión con Google y un sistema de impersonación para administradores.

Este documento explica cómo ejecutar tanto el backend (ASP.NET Core) como el frontend (React + Vite) por primera vez.

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
│   ├── Controllers/
│   ├── Data/
│   ├── Helpers/
│   ├── Models/
│   ├── appsettings.json
│   └── Proyectofinalpow2.csproj
└── Frontend/
    ├── src/
    ├── index.html
    ├── package.json
    └── vite.config.js

---

## Requisitos previos

Instalar:

- .NET SDK 8
- Node.js 18 o superior
- MySQL Server 8

---

## Configuración de la base de datos MySQL

Crear la base de datos:

```sql
CREATE DATABASE unitask CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

---

# Backend – Ejecución inicial

## 1. Entrar al backend

```bash
cd Backend
```

## 2. Configurar appsettings.json

```json
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
  }
}
```

## 3. Ejecutar backend

```bash
dotnet run
```

---

# Frontend – Ejecución inicial

## 1. Entrar al frontend

```bash
cd Frontend
```

## 2. Instalar dependencias

```bash
npm install
```

## 3. Ejecutar frontend

```bash
npm run dev
```

Abrir en el navegador:

```
http://localhost:5173/
```

---

## Conexión frontend-backend

Debe cumplirse:

- API en http://localhost:5000
- Frontend en http://localhost:5173

Asegurar uso de:

```js
const API_BASE = "http://localhost:5000";
```

---

Archivo generado automáticamente.
