using ContaditoAuthBackend.Data;
using ContaditoAuthBackend.Helpers;
using ContaditoAuthBackend.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ContaditoAuthBackend.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _cfg;

        public AuthController(ApplicationDbContext db, IConfiguration cfg)
        {
            _db = db;
            _cfg = cfg;
        }

        public class GoogleLoginDto
        {
            public string Credential { get; set; } = string.Empty;
        }

        public class RegisterDto
        {
            public string Nombre { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginDto
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class ImpersonarDto
        {
            public Guid UsuarioId { get; set; }
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var email = (dto.Email ?? string.Empty).Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Correo y contraseña son requeridos" });

            var exists = await _db.Usuarios.AnyAsync(u => u.Email == email);
            if (exists)
                return Conflict(new { message = "Ya existe un usuario registrado con ese correo" });

            var user = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre?.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Timezone = "America/Managua",
                Locale = "es",
                AuthProvider = "local",
                GoogleSub = null,
                EmailVerified = false,
                Rol = "usuario",
                CreadoEn = DateTime.UtcNow,
                ActualizadoEn = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            _db.Usuarios.Add(user);
            await _db.SaveChangesAsync();

            var jwt = JwtHelper.CreateJwt(user, _cfg);

            Response.Cookies.Append("auth_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                token = jwt,
                user = new
                {
                    id = user.Id,
                    nombre = user.Nombre,
                    email = user.Email,
                    provider = user.AuthProvider,
                    rol = user.Rol
                }
            });
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var email = (dto.Email ?? string.Empty).Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(dto.Password))
                return Unauthorized(new { message = "Credenciales inválidas" });

            var user = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.AuthProvider == "local");

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return Unauthorized(new { message = "Correo o contraseña incorrectos" });

            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok)
                return Unauthorized(new { message = "Correo o contraseña incorrectos" });

            user.LastLogin = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var jwt = JwtHelper.CreateJwt(user, _cfg);

            Response.Cookies.Append("auth_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                token = jwt,
                user = new
                {
                    id = user.Id,
                    nombre = user.Nombre,
                    email = user.Email,
                    provider = user.AuthProvider,
                    rol = user.Rol
                }
            });
        }

        [HttpPost("google")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Credential))
                return Unauthorized("Missing credential");

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = _cfg.GetSection("GoogleAuth:ClientIds").Get<string[]?>()
            };

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.Credential, settings);
            }
            catch
            {
                return Unauthorized("Invalid Google token");
            }

            var googleSub = payload.Subject;
            var email = (payload.Email ?? string.Empty).Trim().ToLower();

            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.GoogleSub == googleSub);

            if (user == null && !string.IsNullOrEmpty(email))
            {
                user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            }

            if (user == null)
            {
                user = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nombre = payload.Name,
                    Email = email,
                    PasswordHash = string.Empty,
                    Timezone = "America/Managua",
                    Locale = "es",
                    AuthProvider = "google",
                    GoogleSub = googleSub,
                    EmailVerified = payload.EmailVerified,
                    Rol = "usuario",
                    CreadoEn = DateTime.UtcNow,
                    ActualizadoEn = DateTime.UtcNow
                };
                _db.Usuarios.Add(user);
            }
            else
            {
                user.AuthProvider = "google";
                user.GoogleSub ??= googleSub;
                user.EmailVerified = payload.EmailVerified;
                if (string.IsNullOrWhiteSpace(user.Nombre))
                    user.Nombre = payload.Name;
                user.ActualizadoEn = DateTime.UtcNow;
            }

            user.LastLogin = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var jwt = JwtHelper.CreateJwt(user, _cfg);

            Response.Cookies.Append("auth_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                token = jwt,
                user = new
                {
                    id = user.Id,
                    nombre = user.Nombre,
                    email = user.Email,
                    provider = user.AuthProvider,
                    rol = user.Rol
                }
            });
        }

        [HttpPost("impersonar")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Impersonar([FromBody] ImpersonarDto dto)
        {
            var uidStr = User.FindFirstValue("uid");
            var rol = User.FindFirstValue("rol") ?? "usuario";

            if (!Guid.TryParse(uidStr, out var adminId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            if (rol != "admin")
                return Forbid();

            var target = await _db.Usuarios.FindAsync(dto.UsuarioId);
            if (target == null)
                return NotFound(new { message = "Usuario destino no existe" });

            var jwt = JwtHelper.CreateJwt(target, _cfg, impersonatedBy: adminId);

            Response.Cookies.Append("auth_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            return Ok(new
            {
                token = jwt,
                user = new
                {
                    id = target.Id,
                    nombre = target.Nombre,
                    email = target.Email,
                    provider = target.AuthProvider,
                    rol = target.Rol
                }
            });
        }
    }
}
