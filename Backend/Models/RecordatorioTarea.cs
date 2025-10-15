using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContaditoAuthBackend.Models
{
    [Table("recordatorio_tarea")] // 👈 nombre real en MySQL
    public class RecordatorioTarea
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("tarea_id")]
        public Guid TareaId { get; set; }

        [Column("minutos_antes")]
        public int MinutosAntes { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("enviado_en")]
        public DateTime? EnviadoEn { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        // Navegación (opcional)
        public Tarea Tarea { get; set; } = null!;
    }
}
