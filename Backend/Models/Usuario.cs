using System;

namespace ContaditoAuthBackend.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string? Nombre { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Timezone { get; set; } = "America/Managua";
        public string Locale { get; set; } = "es";
        public DateTime? CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public string AuthProvider { get; set; } = "local";
        public string? GoogleSub { get; set; }
        public bool EmailVerified { get; set; }

        // 👇 NUEVO
        public string Rol { get; set; } = "usuario";

        public DateTime? LastLogin { get; set; }
    }
}
