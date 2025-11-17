using System.Text;
using System.Security.Claims;
using System.Linq; // 👈 agregá esto
using ContaditoAuthBackend.Data;
using ContaditoAuthBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ===== CORS =====
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]?>()
                   ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("frontend", p => p
        .WithOrigins(corsOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            RoleClaimType = ClaimTypes.Role // 👈 para [Authorize(Roles="admin")]
        };

        opts.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                // 1) Si viene Authorization: Bearer xxx, dejamos que el handler lo use
                var authHeader = ctx.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader))
                {
                    // No tocamos ctx.Token, el JWT handler ya sabe leer el header
                    return Task.CompletedTask;
                }

                // 2) Si NO hay header, usamos la cookie auth_token (para navegación normal)
                if (ctx.Request.Cookies.TryGetValue("auth_token", out var token) &&
                    !string.IsNullOrWhiteSpace(token))
                {
                    ctx.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Dejalo default
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Worker de recordatorios
builder.Services.AddHostedService<ReminderWorker>();

var app = builder.Build();

app.UseCors("frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
