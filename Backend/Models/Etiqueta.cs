using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaditoAuthBackend.Models
{
    [Table("etiqueta")]
    public class Etiqueta
    {
      [Key]
      [Column("id")]
      public Guid Id { get; set; }

      [Required]
      [Column("usuario_id")]
      public Guid UsuarioId { get; set; }

      [Required]
      [Column("nombre")]
      [MaxLength(100)]
      public string Nombre { get; set; } = "";

      [Column("color_hex")]
      [MaxLength(7)]
      public string? ColorHex { get; set; }

      [Column("creado_en")]
      public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

      [Column("actualizado_en")]
      public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

      // Nav
      public Usuario? Usuario { get; set; }
      public ICollection<TareaEtiqueta> TareaEtiquetas { get; set; } = new List<TareaEtiqueta>();
    }
}
