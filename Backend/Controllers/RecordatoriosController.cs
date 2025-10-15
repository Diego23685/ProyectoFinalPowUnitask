// Controllers/RecordatoriosController.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using ContaditoAuthBackend.Data;          // 👈 ajusta si tu DbContext está en otro namespace
using ContaditoAuthBackend.Models;        // 👈 donde está RecordatorioTarea / Tarea / Materia / Usuario

namespace ContaditoAuthBackend.Controllers  // 👈 usa el namespace real de tus otros controllers
{
    [ApiController]
    [Route("tareas/{tareaId:guid}/recordatorios")]
    [Authorize]
    public class RecordatoriosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public RecordatoriosController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> List(Guid tareaId)
        {
            var rs = await _db.Recordatorios
                .Where(r => r.TareaId == tareaId && r.Activo)
                .OrderBy(r => r.MinutosAntes)
                .Select(r => new {
                    id = r.Id,
                    minutos_antes = r.MinutosAntes,
                    activo = r.Activo,
                    enviado_en = r.EnviadoEn,
                    creado_en = r.CreadoEn
                })
                .ToListAsync();

            return Ok(rs);
        }

        public record CreateDto(int Minutos_antes);

        [HttpPost]
        public async Task<IActionResult> Create(Guid tareaId, [FromBody] CreateDto dto)
        {
            if (dto.Minutos_antes <= 0)
                return BadRequest(new { message = "minutos_antes > 0" });

            var exists = await _db.Tareas.AnyAsync(t => t.Id == tareaId);
            if (!exists) return NotFound();

            var r = new RecordatorioTarea
            {
                Id = Guid.NewGuid(),
                TareaId = tareaId,
                MinutosAntes = dto.Minutos_antes,
                Activo = true
            };

            _db.Recordatorios.Add(r);
            await _db.SaveChangesAsync();

            return Ok(new {
                id = r.Id,
                minutos_antes = r.MinutosAntes,
                activo = r.Activo,
                enviado_en = r.EnviadoEn
            });
        }
    }

    [ApiController]
    [Route("recordatorios")]
    [Authorize]
    public class RecordatorioOpsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public RecordatorioOpsController(ApplicationDbContext db) => _db = db;

        public record PatchDto(bool? Activo);

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] PatchDto dto)
        {
            var r = await _db.Recordatorios.FindAsync(id);
            if (r == null) return NotFound();

            if (dto.Activo.HasValue) r.Activo = dto.Activo.Value;

            await _db.SaveChangesAsync();
            return Ok(new { id = r.Id, activo = r.Activo });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var r = await _db.Recordatorios.FindAsync(id);
            if (r == null) return NotFound();

            _db.Recordatorios.Remove(r);
            await _db.SaveChangesAsync();
            return Ok(new { ok = true });
        }
    }
}
