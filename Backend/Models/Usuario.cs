using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaditoAuthBackend.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Required]
        [Column("email")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("timezone")]
        public string Timezone { get; set; } = "America/Managua";

        [Column("locale")]
        public string Locale { get; set; } = "es";

        [Column("auth_provider")]
        public string AuthProvider { get; set; } = "local";

        [Column("google_sub")]
        public string? GoogleSub { get; set; }

        [Column("email_verified")]
        public bool EmailVerified { get; set; } = false;

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [Column("actualizado_en")]
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;
    }
}
