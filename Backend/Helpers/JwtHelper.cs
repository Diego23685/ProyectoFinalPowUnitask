// Helpers/JwtHelper.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ContaditoAuthBackend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ContaditoAuthBackend.Helpers
{
    public static class JwtHelper
    {
        public static string CreateJwt(Usuario user, IConfiguration cfg, Guid? impersonatedBy = null)
        {
            var key = cfg["Jwt:Key"] ?? "dev-key-change-me";
            var issuer = cfg["Jwt:Issuer"];
            var audience = cfg["Jwt:Audience"];

            var claims = new List<Claim>
            {
                // ====== IDs ======
                // Claim custom que ya usabas
                new Claim("uid", user.Id.ToString()),
                // Claim estándar que usan muchos middlewares
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                // ====== Email / nombre ======
                new Claim("email", user.Email ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Nombre ?? user.Email ?? string.Empty),

                // ====== Rol ======
                new Claim("rol", user.Rol ?? "usuario"),
                // Para que [Authorize(Roles = "admin")] funcione
                new Claim(ClaimTypes.Role, user.Rol ?? "usuario")
            };

            if (impersonatedBy.HasValue)
            {
                // Quién está impersonando a este usuario
                claims.Add(new Claim("imp_by", impersonatedBy.Value.ToString()));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
