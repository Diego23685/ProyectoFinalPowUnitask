using ContaditoAuthBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ContaditoAuthBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<RecordatorioTarea> Recordatorios { get; set; }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Materia> Materias => Set<Materia>();
        public DbSet<Tarea> Tareas => Set<Tarea>();
        public DbSet<Etiqueta> Etiquetas => Set<Etiqueta>();
        public DbSet<TareaEtiqueta> TareasEtiquetas => Set<TareaEtiqueta>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuario
            var u = modelBuilder.Entity<Usuario>();
            u.HasIndex(x => x.Email).IsUnique();
            u.HasIndex(x => x.GoogleSub);
            u.Property(x => x.Id).HasColumnType("char(36)");

            // Materia
            var m = modelBuilder.Entity<Materia>();
            m.Property(x => x.Id).HasColumnType("char(36)");
            m.Property(x => x.UsuarioId).HasColumnType("char(36)");
            m.HasOne(x => x.Usuario)
              .WithMany()
              .HasForeignKey(x => x.UsuarioId)
              .OnDelete(DeleteBehavior.Cascade);
            m.HasIndex(x => new { x.UsuarioId, x.Nombre }).IsUnique();

            // Tarea
            var t = modelBuilder.Entity<Tarea>();
            t.Property(x => x.Id).HasColumnType("char(36)");
            t.Property(x => x.MateriaId).HasColumnType("char(36)");
            t.HasOne(x => x.Materia)
              .WithMany(x => x.Tareas)
              .HasForeignKey(x => x.MateriaId)
              .OnDelete(DeleteBehavior.Cascade);

            // Etiqueta
            var e = modelBuilder.Entity<Etiqueta>();
            e.Property(x => x.Id).HasColumnType("char(36)");
            e.Property(x => x.UsuarioId).HasColumnType("char(36)");
            e.HasOne(x => x.Usuario)
              .WithMany()
              .HasForeignKey(x => x.UsuarioId)
              .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.UsuarioId, x.Nombre }).IsUnique();

            // TareaEtiqueta (PK compuesta)
            var te = modelBuilder.Entity<TareaEtiqueta>();
            te.HasKey(x => new { x.TareaId, x.EtiquetaId });
            te.Property(x => x.TareaId).HasColumnType("char(36)");
            te.Property(x => x.EtiquetaId).HasColumnType("char(36)");
            te.HasOne(x => x.Tarea)
              .WithMany(x => x.TareaEtiquetas)
              .HasForeignKey(x => x.TareaId)
              .OnDelete(DeleteBehavior.Cascade);
            te.HasOne(x => x.Etiqueta)
              .WithMany(x => x.TareaEtiquetas)
              .HasForeignKey(x => x.EtiquetaId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
