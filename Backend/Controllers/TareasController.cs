// Controllers/TareasController.cs
using ContaditoAuthBackend.Data;
using ContaditoAuthBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContaditoAuthBackend.Controllers
{
    [ApiController]
    [Route("tareas")]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public TareasController(ApplicationDbContext db) { _db = db; }

        // ====== DTOs ======
        public class CreateTareaDto
        {
            public Guid Materia_id { get; set; }
            public string Titulo { get; set; } = "";
            public string? Descripcion { get; set; }
            public DateTime Vence_en { get; set; }
            public string Prioridad { get; set; } = "Media";
        }

        // DTO para PATCH: todo opcional
        public class PatchTareaDto
        {
            public string? Titulo { get; set; }
            public string? Descripcion { get; set; }
            public DateTime? Vence_en { get; set; }
            public string? Prioridad { get; set; }   // "Alta" | "Media" | "Baja"
            public bool? Completada { get; set; }
            public bool? Silenciada { get; set; }
            public bool? Eliminada { get; set; }
        }

        // ====== LIST ======
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Guid usuario_id)
        {
            var materiaIds = await _db.Materias
                .Where(m => m.UsuarioId == usuario_id && m.Estado == "activo")
                .Select(m => m.Id)
                .ToListAsync();

            var tareas = await _db.Tareas
                .Where(t => materiaIds.Contains(t.MateriaId) && !t.Eliminada)
                .OrderBy(t => t.VenceEn)
                .ToListAsync();

            var tareaIds = tareas.Select(t => t.Id).ToList();
            var te = await _db.TareasEtiquetas
                .Where(x => tareaIds.Contains(x.TareaId))
                .Include(x => x.Etiqueta)
                .ToListAsync();

            var etiquetasPorTarea = te
                .GroupBy(x => x.TareaId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new EtiquetaOut
                    {
                        id = x.EtiquetaId,
                        nombre = x.Etiqueta!.Nombre,
                        color_hex = x.Etiqueta!.ColorHex
                    }).ToList()
                );

            var result = tareas.Select(t => new
            {
                id = t.Id,
                materia_id = t.MateriaId,
                titulo = t.Titulo,
                descripcion = t.Descripcion,
                vence_en = t.VenceEn,
                prioridad = t.Prioridad,
                completada = t.Completada,
                silenciada = t.Silenciada,
                eliminada = t.Eliminada,
                creado_en = t.CreadoEn,
                actualizado_en = t.ActualizadoEn,
                etiquetas = etiquetasPorTarea.TryGetValue(t.Id, out var list) ? list : new List<EtiquetaOut>()
            });

            return Ok(result);
        }

        private class EtiquetaOut
        {
            public Guid id { get; set; }
            public string nombre { get; set; } = "";
            public string? color_hex { get; set; }
        }

        // ====== CREATE ======
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTareaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Titulo))
                return BadRequest(new { message = "Título requerido" });

            var m = await _db.Materias.FindAsync(dto.Materia_id);
            if (m == null || m.Estado != "activo")
                return BadRequest(new { message = "Materia inválida" });

            var t = new Tarea
            {
                Id = Guid.NewGuid(),
                MateriaId = dto.Materia_id,
                Titulo = dto.Titulo.Trim(),
                Descripcion = dto.Descripcion,
                VenceEn = dto.Vence_en,
                Prioridad = dto.Prioridad,
                Completada = false,
                Silenciada = false,
                Eliminada = false,
                CreadoEn = DateTime.UtcNow,
                ActualizadoEn = DateTime.UtcNow
            };
            _db.Tareas.Add(t);
            await _db.SaveChangesAsync();

            var outDto = new
            {
                id = t.Id,
                materia_id = t.MateriaId,
                titulo = t.Titulo,
                descripcion = t.Descripcion,
                vence_en = t.VenceEn,
                prioridad = t.Prioridad,
                completada = t.Completada,
                silenciada = t.Silenciada,
                eliminada = t.Eliminada,
                creado_en = t.CreadoEn,
                actualizado_en = t.ActualizadoEn,
                etiquetas = new List<object>()
            };

            return Ok(outDto);
        }

        // ====== PATCH (parcial, sin validación obligatoria) ======
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] PatchTareaDto dto)
        {
            var t = await _db.Tareas.FindAsync(id);
            if (t == null) return NotFound();

            if (dto.Titulo != null) t.Titulo = dto.Titulo.Trim();
            if (dto.Descripcion != null) t.Descripcion = dto.Descripcion;
            if (dto.Vence_en.HasValue) t.VenceEn = dto.Vence_en.Value;
            if (dto.Prioridad != null) t.Prioridad = dto.Prioridad;
            if (dto.Completada.HasValue) t.Completada = dto.Completada.Value;
            if (dto.Silenciada.HasValue) t.Silenciada = dto.Silenciada.Value;
            if (dto.Eliminada.HasValue) t.Eliminada = dto.Eliminada.Value;

            t.ActualizadoEn = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var outDto = new
            {
                id = t.Id,
                materia_id = t.MateriaId,
                titulo = t.Titulo,
                descripcion = t.Descripcion,
                vence_en = t.VenceEn,
                prioridad = t.Prioridad,
                completada = t.Completada,
                silenciada = t.Silenciada,
                eliminada = t.Eliminada,
                creado_en = t.CreadoEn,
                actualizado_en = t.ActualizadoEn,
                etiquetas = new List<object>() // si necesitas, carga etiquetas igual que en GET
            };

            return Ok(outDto);
        }

        // ====== etiquetas N:M ======
        [HttpPost("{id:guid}/etiquetas/{etiquetaId:guid}")]
        public async Task<IActionResult> AddEtiqueta(Guid id, Guid etiquetaId)
        {
            var t = await _db.Tareas.FindAsync(id);
            var e = await _db.Etiquetas.FindAsync(etiquetaId);
            if (t == null || e == null) return NotFound();

            var exists = await _db.TareasEtiquetas.AnyAsync(x => x.TareaId == id && x.EtiquetaId == etiquetaId);
            if (!exists)
            {
                _db.TareasEtiquetas.Add(new TareaEtiqueta { TareaId = id, EtiquetaId = etiquetaId });
                await _db.SaveChangesAsync();
            }
            return Ok(new { ok = true });
        }

        // DELETE /tareas/{id}  -> soft delete (eliminada=true)
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var t = await (from tarea in _db.Tareas
                           join m in _db.Materias on tarea.MateriaId equals m.Id
                           // opcional: verifica que la tarea pertenece al usuario autenticado
                           // where m.UsuarioId == userIdActual
                           where tarea.Id == id
                           select tarea)
                          .FirstOrDefaultAsync();

            if (t == null) return NotFound();

            // Soft delete
            t.Eliminada = true;
            await _db.SaveChangesAsync();

            return Ok(new { ok = true, id = t.Id, eliminada = t.Eliminada });
        }
    }
}
