using System.Text;
using ContaditoAuthBackend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ContaditoAuthBackend.Services; // 👈 para ver ReminderWorker


var builder = WebApplication.CreateBuilder(args);

// ===== CORS =====
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]?>()
                   ?? new[] { "http://localhost:3000" };
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("frontend", p => p
        .WithOrigins(corsOrigins)    // nada de "*"
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()          // si usas cookie httpOnly
    );
});

// ===== DbContext (MySQL) =====
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    options.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

// ===== JWT Auth =====
var key = builder.Configuration["Jwt:Key"] ?? "dev-key-change-me";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        // Permitir token desde cookie httpOnly "auth_token"
        opts.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (ctx.Request.Cookies.TryGetValue("auth_token", out var token))
                    ctx.Token = token;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Config por si quieres controlar fechas, etc. (por defecto serializa DateTime en ISO)
        // o.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ReminderWorker>();

var app = builder.Build();

// ===== CORS antes que todo lo que reciba requests =====
app.UseCors("frontend");

// ===== Swagger (solo en Dev) =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===== DB bootstrap rápido (dev) =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated(); // para prod usa Migraciones
}

// HTTPS redirection (evita warning configurando launchSettings o quítalo en dev)
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
