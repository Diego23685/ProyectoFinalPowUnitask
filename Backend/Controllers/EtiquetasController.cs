using System.Security.Claims;
using ContaditoAuthBackend.Data;
using ContaditoAuthBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContaditoAuthBackend.Controllers
{
    [ApiController]
    [Route("etiquetas")]
    public class EtiquetasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public EtiquetasController(ApplicationDbContext db) { _db = db; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> List([FromQuery] Guid usuario_id)
        {
            var uidStr = User.FindFirstValue("uid");
            var rol = User.FindFirstValue("rol") ?? "usuario";

            if (!Guid.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            if (rol != "admin" && userId != usuario_id)
                return Forbid();

            var list = await _db.Etiquetas
                .Where(e => e.UsuarioId == usuario_id)
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("mias")]
        [Authorize]
        public async Task<IActionResult> ListMine()
        {
            var uidStr = User.FindFirstValue("uid");
            if (!Guid.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            var list = await _db.Etiquetas
                .Where(e => e.UsuarioId == userId)
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            return Ok(list);
        }

        public class CreateEtiquetaDto
        {
            public Guid UsuarioId { get; set; }
            public string Nombre { get; set; } = "";
            public string? ColorHex { get; set; }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateEtiquetaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "Nombre requerido" });

            var uidStr = User.FindFirstValue("uid");
            var rol = User.FindFirstValue("rol") ?? "usuario";

            if (!Guid.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            if (rol != "admin")
                dto.UsuarioId = userId;

            var exists = await _db.Etiquetas.AnyAsync(e =>
                e.UsuarioId == dto.UsuarioId && e.Nombre == dto.Nombre);

            if (exists)
                return Conflict(new { message = "Ya existe una etiqueta con ese nombre" });

            var e = new Etiqueta
            {
                Id = Guid.NewGuid(),
                UsuarioId = dto.UsuarioId,
                Nombre = dto.Nombre.Trim(),
                ColorHex = dto.ColorHex,
                CreadoEn = DateTime.UtcNow,
                ActualizadoEn = DateTime.UtcNow
            };

            _db.Etiquetas.Add(e);
            await _db.SaveChangesAsync();

            return Ok(e);
        }
    }
}
