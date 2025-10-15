using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaditoAuthBackend.Models
{
    [Table("tarea_etiqueta")]
    public class TareaEtiqueta
    {
      [Column("tarea_id")]
      public Guid TareaId { get; set; }

      [Column("etiqueta_id")]
      public Guid EtiquetaId { get; set; }

      [Column("creado_en")]
      public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

      // Nav
      public Tarea? Tarea { get; set; }
      public Etiqueta? Etiqueta { get; set; }
    }
}
