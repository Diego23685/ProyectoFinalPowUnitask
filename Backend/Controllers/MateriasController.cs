using System.Security.Claims;
using ContaditoAuthBackend.Data;
using ContaditoAuthBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContaditoAuthBackend.Controllers
{
    [ApiController]
    [Route("materias")]
    public class MateriasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public MateriasController(ApplicationDbContext db) { _db = db; }

        public class CreateMateriaDto
        {
            public string Nombre { get; set; } = "";
        }

        public class UpdateMateriaDto
        {
            public string? Nombre { get; set; }
            public string? Estado { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Guid usuario_id)
        {
            var materias = await _db.Materias
                .Where(m => m.UsuarioId == usuario_id && m.Estado == "activo")
                .OrderByDescending(m => m.CreadoEn)
                .ToListAsync();

            return Ok(materias);
        }

        [HttpGet("mias")]
        [Authorize]
        public async Task<IActionResult> ListMine()
        {
            var uidStr = User.FindFirstValue("uid");
            if (!Guid.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            var materias = await _db.Materias
                .Where(m => m.UsuarioId == userId && m.Estado == "activo")
                .OrderByDescending(m => m.CreadoEn)
                .ToListAsync();

            return Ok(materias);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateMateriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "Nombre requerido" });

            var uidStr = User.FindFirstValue("uid");
            if (!Guid.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            var nombre = dto.Nombre.Trim();

            var exists = await _db.Materias.AnyAsync(m =>
                m.UsuarioId == userId &&
                m.Nombre == nombre &&
                m.Estado == "activo");

            if (exists)
                return Conflict(new { message = "Ya existe una materia con ese nombre" });

            var m = new Materia
            {
                Id = Guid.NewGuid(),
                UsuarioId = userId,
                Nombre = nombre,
                Estado = "activo",
                CreadoEn = DateTime.UtcNow,
                ActualizadoEn = DateTime.UtcNow
            };

            _db.Materias.Add(m);
            await _db.SaveChangesAsync();
            return Ok(m);
        }

        [HttpPatch("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateMateriaDto dto)
        {
            var uidStr = User.FindFirstValue("uid");
            if (!Guid.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            var materia = await _db.Materias
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == userId);

            if (materia == null)
                return NotFound(new { message = "Materia no encontrada" });

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
            {
                var nuevoNombre = dto.Nombre.Trim();

                var nameExists = await _db.Materias.AnyAsync(m =>
                    m.UsuarioId == userId &&
                    m.Id != id &&
                    m.Nombre == nuevoNombre &&
                    m.Estado == "activo");

                if (nameExists)
                    return Conflict(new { message = "Ya existe otra materia con ese nombre" });

                materia.Nombre = nuevoNombre;
            }

            if (!string.IsNullOrWhiteSpace(dto.Estado))
            {
                materia.Estado = dto.Estado.Trim().ToLower() == "eliminada"
                    ? "eliminada"
                    : dto.Estado.Trim();
            }

            materia.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(materia);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var uidStr = User.FindFirstValue("uid");
            if (!Guid.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            var materia = await _db.Materias
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == userId);

            if (materia == null)
                return NotFound(new { message = "Materia no encontrada" });

            materia.Estado = "eliminada";
            materia.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(new { message = "Materia eliminada", materia.Id });
        }
    }
}
