using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetCore.API.Middleware;
using PetCore.Domain.Interfaces;
using PetCore.Infrastructure.Data;
using PetCore.Infrastructure.Data.Seed;
using PetCore.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Tenant Provider + HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<AuditoriaInterceptor>();

// DbContext com interceptor de auditoria
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    var tenant = sp.GetService<ITenantProvider>();
    if (tenant?.UsuarioId != null)
        options.AddInterceptors(sp.GetRequiredService<AuditoriaInterceptor>());
});

// Autenticação JWT
var jwtSecret = builder.Configuration["JwtSettings:Secret"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped<IServicoAutenticacao, ServicoAutenticacao>();
builder.Services.AddScoped<IServicoAuditoria, ServicoAuditoria>();
builder.Services.AddScoped<ServicoClinica>();
builder.Services.AddScoped<ServicoUsuario>();
builder.Services.AddScoped<ServicoEspecie>();
builder.Services.AddScoped<ServicoTutor>();
builder.Services.AddScoped<ServicoPaciente>();
builder.Services.AddScoped<ServicoAgendamento>();
builder.Services.AddScoped<ServicoProntuario>();
builder.Services.AddScoped<ServicoInternacao>();
builder.Services.AddScoped<ServicoExame>();
builder.Services.AddScoped<ServicoProduto>();
builder.Services.AddScoped<ServicoMovimentacao>();
builder.Services.AddScoped<ServicoPedido>();
builder.Services.AddScoped<ServicoFinanceiro>();
builder.Services.AddScoped<ServicoCentroCusto>();
builder.Services.AddScoped<ServicoDashboard>();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// CORS configurável via variável de ambiente
var corsOrigins = builder.Configuration["CORS_ORIGINS"] ?? "http://localhost:3000";
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries))
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pipeline de middlewares
app.UseTratamentoExcecoes();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCore API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("PermitirFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseClinicaTenant();

app.MapControllers();

// Aplicar migrations e seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SemeadorDados.SemearAsync(db);
}

await app.RunAsync();
