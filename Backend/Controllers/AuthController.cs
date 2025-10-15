using ContaditoAuthBackend.Data;
using ContaditoAuthBackend.Helpers;
using ContaditoAuthBackend.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

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
            public string Credential { get; set; } = string.Empty; // id_token
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

            // 1) Buscar por google_sub
            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.GoogleSub == googleSub);

            // 2) Vincular por email si ya existe localmente
            if (user == null && !string.IsNullOrEmpty(email))
            {
                user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            }

            // 3) Crear si no existe
            if (user == null)
            {
                user = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nombre = payload.Name,
                    Email = email,
                    PasswordHash = string.Empty,          // no requerido para Google
                    Timezone = "America/Managua",
                    Locale = "es",
                    AuthProvider = "google",
                    GoogleSub = googleSub,
                    EmailVerified = payload.EmailVerified, // <-- bool directo
                    CreadoEn = DateTime.UtcNow,
                    ActualizadoEn = DateTime.UtcNow
                };
                _db.Usuarios.Add(user);
            }
            else
            {
                // Actualizar metadata y enlazar Google si faltaba
                user.AuthProvider = "google";
                user.GoogleSub ??= googleSub;
                user.EmailVerified = payload.EmailVerified; // <-- bool directo
                if (string.IsNullOrWhiteSpace(user.Nombre))
                    user.Nombre = payload.Name;
                user.ActualizadoEn = DateTime.UtcNow;
            }

            user.LastLogin = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var jwt = JwtHelper.CreateJwt(user, _cfg);

            // Cookie HTTP-Only opcional (útil para web)
            Response.Cookies.Append("auth_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                 // en prod debe ser true
                SameSite = SameSiteMode.None,  // si el front está en otro dominio
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                token = jwt,
                user = new { id = user.Id, nombre = user.Nombre, email = user.Email, provider = user.AuthProvider }
            });
        }
    }
}
