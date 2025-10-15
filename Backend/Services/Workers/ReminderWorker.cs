// Services/Workers/ReminderWorker.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ContaditoAuthBackend.Data;

namespace ContaditoAuthBackend.Services   // 👈 Debe coincidir con el using en Program.cs
{
    public class ReminderWorker : BackgroundService   // 👈 public
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<ReminderWorker> _log;

        public ReminderWorker(IServiceProvider sp, ILogger<ReminderWorker> log)
        {
            _sp = sp;
            _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var nowUtc = DateTime.UtcNow;

                    var due = await (from r in db.Recordatorios
                                     join t in db.Tareas on r.TareaId equals t.Id
                                     join m in db.Materias on t.MateriaId equals m.Id
                                     join u in db.Usuarios on m.UsuarioId equals u.Id
                                     where r.Activo && r.EnviadoEn == null && !t.Eliminada && !t.Completada
                                     let remindUtc = t.VenceEn.AddMinutes(-r.MinutosAntes)
                                     where remindUtc <= nowUtc
                                     select new { r, t, u }).ToListAsync(stoppingToken);

                    foreach (var x in due)
                    {
                        _log.LogInformation("Recordatorio: {Email} -> {Titulo}", x.u.Email, x.t.Titulo);
                        x.r.EnviadoEn = nowUtc;
                    }
                    if (due.Count > 0) await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "ReminderWorker error");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
