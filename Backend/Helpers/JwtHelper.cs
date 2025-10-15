using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ContaditoAuthBackend.Models;
using Microsoft.IdentityModel.Tokens;

namespace ContaditoAuthBackend.Helpers
{
    public static class JwtHelper
    {
        public static string CreateJwt(Usuario user, IConfiguration cfg)
        {
            var issuer = cfg["Jwt:Issuer"];
            var audience = cfg["Jwt:Audience"];
            var key = cfg["Jwt:Key"] ?? "dev-key-change-me";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("uid", user.Id.ToString()),            // 👈 importante
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.Nombre ?? string.Empty),
                new Claim("provider", user.AuthProvider ?? "local")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
