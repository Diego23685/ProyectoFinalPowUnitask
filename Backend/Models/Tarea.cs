using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaditoAuthBackend.Models
{
    [Table("tarea")]
    public class Tarea
    {
      [Key]
      [Column("id")]
      public Guid Id { get; set; }

      [Required]
      [Column("materia_id")]
      public Guid MateriaId { get; set; }

      [Required]
      [Column("titulo")]
      [MaxLength(150)]
      public string Titulo { get; set; } = "";

      [Column("descripcion")]
      public string? Descripcion { get; set; }

      [Required]
      [Column("vence_en")]
      public DateTime VenceEn { get; set; }

      [Required]
      [Column("prioridad")]
      public string Prioridad { get; set; } = "Media"; // Alta|Media|Baja

      [Column("completada")]
      public bool Completada { get; set; } = false;

      [Column("silenciada")]
      public bool Silenciada { get; set; } = false;

      [Column("eliminada")]
      public bool Eliminada { get; set; } = false;

      [Column("creado_en")]
      public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

      [Column("actualizado_en")]
      public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

      // Nav
      public Materia? Materia { get; set; }
      public ICollection<TareaEtiqueta> TareaEtiquetas { get; set; } = new List<TareaEtiqueta>();
    }
}
