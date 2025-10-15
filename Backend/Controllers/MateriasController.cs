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

        // --------- DTOs ----------
        public class CreateMateriaDto { public string Nombre { get; set; } = ""; }

        // GET /materias?usuario_id=GUID  (legacy: sigue soportado)
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Guid usuario_id)
        {
            var materias = await _db.Materias
                .Where(m => m.UsuarioId == usuario_id)
                .OrderByDescending(m => m.CreadoEn)
                .ToListAsync();

            return Ok(materias);
        }

        // GET /materias/mias  (seguro: usa uid del token)
        [HttpGet("mias")]
        [Authorize]
        public async Task<IActionResult> ListMine()
        {
            var uidStr = User.FindFirstValue("uid");
            if (!Guid.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            var materias = await _db.Materias
                .Where(m => m.UsuarioId == userId)
                .OrderByDescending(m => m.CreadoEn)
                .ToListAsync();

            return Ok(materias);
        }

        // POST /materias  (crea usando uid del token)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateMateriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "Nombre requerido" });

            var uidStr = User.FindFirstValue("uid");
            if (!Guid.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Token inválido (sin uid)" });

            var exists = await _db.Materias.AnyAsync(m =>
                m.UsuarioId == userId && m.Nombre == dto.Nombre && m.Estado == "activo");

            if (exists)
                return Conflict(new { message = "Ya existe una materia con ese nombre" });

            var m = new Materia
            {
                Id = Guid.NewGuid(),
                UsuarioId = userId,
                Nombre = dto.Nombre.Trim(),
                Estado = "activo",
                CreadoEn = DateTime.UtcNow,
                ActualizadoEn = DateTime.UtcNow
            };

            _db.Materias.Add(m);
            await _db.SaveChangesAsync();
            return Ok(m);
        }
    }
}
