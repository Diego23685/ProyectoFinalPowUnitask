using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaditoAuthBackend.Models
{
    [Table("materia")]
    public class Materia
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

      [Required]
      [Column("estado")]
      public string Estado { get; set; } = "activo"; // activo|eliminada

      [Column("creado_en")]
      public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

      [Column("actualizado_en")]
      public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

      // Nav
      public Usuario? Usuario { get; set; }
      public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}
