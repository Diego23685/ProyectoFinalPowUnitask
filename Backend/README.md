# ContaditoAuthBackend

Backend mínimo (.NET 8 + MySQL) con **Google Sign-In** y emisión de **JWT**.
Incluye endpoint `POST /auth/google` que recibe el `id_token` de Google, valida,
crea/actualiza el usuario y devuelve el JWT (además de setear cookie `auth_token` HTTP-Only).

## Requisitos
- .NET 8 SDK
- MySQL 8+
- Google OAuth Client ID (Web)

## Configuración
1. Edita `appsettings.json`:
   - `ConnectionStrings:Default`
   - `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` (usa una clave larga y segura)
   - `GoogleAuth:ClientIds` (puedes poner varios, ej. local y prod)
   - `Cors:Origins` (tu(s) dominio(s) frontend)

2. Crea la base de datos (si no existe):
   - El código usa `EnsureCreated()` para crear el esquema básico al arrancar.

## Ejecutar
```bash
dotnet restore
dotnet run
```
La API abrirá Swagger en `https://localhost:5001/swagger` (o puerto similar).

## Uso del endpoint
`POST /auth/google`
Body JSON:
```json
{ "credential": "ID_TOKEN_DE_GOOGLE" }
```

- Si es válido, devuelve:
```json
{
  "token": "jwt...",
  "user": { "id": "...", "nombre": "...", "email": "...", "provider": "google" }
}
```
y setea cookie `auth_token` con el JWT.

## Notas
- Si ya tenías usuarios con login local y mismo email, el endpoint vincula la cuenta a Google.
- Para producción: usa **HTTPS**, configura CORS correctamente y reemplaza la `Jwt:Key` por una segura.
- Si luego agregas más entidades/tablas, migra a **EF Core Migrations** en lugar de `EnsureCreated()`.
