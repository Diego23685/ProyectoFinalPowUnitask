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

            // ===================== USUARIO =====================
            var u = modelBuilder.Entity<Usuario>();

            u.ToTable("usuario");

            u.Property(x => x.Id)
                .HasColumnType("char(36)")
                .HasColumnName("id");

            u.Property(x => x.Nombre)
                .HasColumnName("nombre");

            u.Property(x => x.Email)
                .HasColumnName("email");

            u.Property(x => x.PasswordHash)
                .HasColumnName("password_hash");

            u.Property(x => x.Timezone)
                .HasColumnName("timezone");

            u.Property(x => x.Locale)
                .HasColumnName("locale");

            u.Property(x => x.CreadoEn)
                .HasColumnName("creado_en");

            u.Property(x => x.ActualizadoEn)
                .HasColumnName("actualizado_en");

            u.Property(x => x.AuthProvider)
                .HasColumnName("auth_provider");

            u.Property(x => x.GoogleSub)
                .HasColumnName("google_sub");

            u.Property(x => x.EmailVerified)
                .HasColumnName("email_verified");

            u.Property(x => x.Rol)
                .HasColumnName("rol");

            u.Property(x => x.LastLogin)
                .HasColumnName("last_login");

            u.HasIndex(x => x.Email).IsUnique();
            u.HasIndex(x => x.GoogleSub).IsUnique();

            // ===================== MATERIA =====================
            var m = modelBuilder.Entity<Materia>();

            m.ToTable("materia");

            m.Property(x => x.Id)
                .HasColumnType("char(36)")
                .HasColumnName("id");

            m.Property(x => x.UsuarioId)
                .HasColumnType("char(36)")
                .HasColumnName("usuario_id");

            m.Property(x => x.Nombre)
                .HasColumnName("nombre");

            m.Property(x => x.Estado)
                .HasColumnName("estado");

            m.Property(x => x.CreadoEn)
                .HasColumnName("creado_en");

            m.Property(x => x.ActualizadoEn)
                .HasColumnName("actualizado_en");

            m.HasOne(x => x.Usuario)
              .WithMany()
              .HasForeignKey(x => x.UsuarioId)
              .OnDelete(DeleteBehavior.Cascade);

            m.HasIndex(x => new { x.UsuarioId, x.Nombre }).IsUnique();

            // ===================== TAREA =====================
            var t = modelBuilder.Entity<Tarea>();

            t.ToTable("tarea");

            t.Property(x => x.Id)
                .HasColumnType("char(36)")
                .HasColumnName("id");

            t.Property(x => x.MateriaId)
                .HasColumnType("char(36)")
                .HasColumnName("materia_id");

            t.Property(x => x.Titulo)
                .HasColumnName("titulo");

            t.Property(x => x.Descripcion)
                .HasColumnName("descripcion");

            t.Property(x => x.VenceEn)
                .HasColumnName("vence_en");

            t.Property(x => x.Prioridad)
                .HasColumnName("prioridad");

            t.Property(x => x.Completada)
                .HasColumnName("completada");

            t.Property(x => x.Silenciada)
                .HasColumnName("silenciada");

            t.Property(x => x.Eliminada)
                .HasColumnName("eliminada");

            t.Property(x => x.CreadoEn)
                .HasColumnName("creado_en");

            t.Property(x => x.ActualizadoEn)
                .HasColumnName("actualizado_en");

            t.HasOne(x => x.Materia)
              .WithMany(x => x.Tareas)
              .HasForeignKey(x => x.MateriaId)
              .OnDelete(DeleteBehavior.Cascade);

            // ===================== ETIQUETA =====================
            var e = modelBuilder.Entity<Etiqueta>();

            e.ToTable("etiqueta");

            e.Property(x => x.Id)
                .HasColumnType("char(36)")
                .HasColumnName("id");

            e.Property(x => x.UsuarioId)
                .HasColumnType("char(36)")
                .HasColumnName("usuario_id");

            e.Property(x => x.Nombre)
                .HasColumnName("nombre");

            e.Property(x => x.ColorHex)
                .HasColumnName("color_hex");

            e.Property(x => x.CreadoEn)
                .HasColumnName("creado_en");

            e.Property(x => x.ActualizadoEn)
                .HasColumnName("actualizado_en");

            e.HasOne(x => x.Usuario)
              .WithMany()
              .HasForeignKey(x => x.UsuarioId)
              .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.UsuarioId, x.Nombre }).IsUnique();

            // ===================== TAREA_ETIQUETA =====================
            var te = modelBuilder.Entity<TareaEtiqueta>();

            te.ToTable("tarea_etiqueta");

            te.HasKey(x => new { x.TareaId, x.EtiquetaId });

            te.Property(x => x.TareaId)
                .HasColumnType("char(36)")
                .HasColumnName("tarea_id");

            te.Property(x => x.EtiquetaId)
                .HasColumnType("char(36)")
                .HasColumnName("etiqueta_id");

            te.Property(x => x.CreadoEn)
                .HasColumnName("creado_en");

            te.HasOne(x => x.Tarea)
              .WithMany(x => x.TareaEtiquetas)
              .HasForeignKey(x => x.TareaId)
              .OnDelete(DeleteBehavior.Cascade);

            te.HasOne(x => x.Etiqueta)
              .WithMany(x => x.TareaEtiquetas)
              .HasForeignKey(x => x.EtiquetaId)
              .OnDelete(DeleteBehavior.Cascade);

            // ===================== RECORDATORIO_TAREA =====================
            var r = modelBuilder.Entity<RecordatorioTarea>();

            r.ToTable("recordatorio_tarea");

            r.Property(x => x.Id)
                .HasColumnType("char(36)")
                .HasColumnName("id");

            r.Property(x => x.TareaId)
                .HasColumnType("char(36)")
                .HasColumnName("tarea_id");

            r.Property(x => x.MinutosAntes)
                .HasColumnName("minutos_antes");

            r.Property(x => x.Activo)
                .HasColumnName("activo");

            r.Property(x => x.EnviadoEn)
                .HasColumnName("enviado_en");

            r.Property(x => x.CreadoEn)
                .HasColumnName("creado_en");
        }
    }
}
