using System.Text;
using DentalClinic.Core.Application.Interfaces;
using DentalClinic.Core.Application.Services;
using DentalClinic.Core.Domain.Repositories;
using DentalClinic.Infrastructure.Persistence;
using DentalClinic.Infrastructure.Persistence.Repositories;
using DentalClinic.Infrastructure.Security;
using DentalClinic.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DentalClinic.Api.Middlewares;
using DentalClinic.Api.Filters;
using DentalClinic.Api.Hubs;
using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.ValueObjects;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados com suporte a JSONB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(dataSource));

// 2. Injeção de Dependências
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IProntuarioRepository, ProntuarioRepository>();
builder.Services.AddScoped<ITreatmentPlanRepository, TreatmentPlanRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IWaitListRepository, WaitListRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAnexoRepository, AnexoRepository>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IStorageService, LocalStorageService>();

builder.Services.AddSignalR();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditFilter>();
});

// 5. Configuração JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 6. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutter", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OdontoClinica API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Swagger configurado para o caminho padrão /swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OdontoClinica API V1");
});

app.UseStaticFiles();
app.UseCors("AllowFlutter");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ClinicHub>("/hubs/clinic");

// 9. Lógica de Seed Robusta (Garante o Admin e loga no console)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var hasher = services.GetRequiredService<IPasswordHasher>();

    try {
        context.Database.Migrate();

        var adminEmail = "admin@odontoclinica.edu.br";
        var user = await context.Users.FirstOrDefaultAsync(u => u.EmailAddress.Value == adminEmail);

        if (user == null)
        {
            user = new User(
                "Administrador Inicial",
                Email.Create(adminEmail),
                hasher.HashPassword("admin123"),
                DentalClinic.Core.Domain.Entities.UserRole.Admin
            );
            user.ConfirmEmail();
            user.Activate();
            context.Users.Add(user);
        }
        else
        {
            user.SetPasswordHash(hasher.HashPassword("admin123"));
            user.Activate();
            context.Users.Update(user);
        }

        await context.SaveChangesAsync();
        // MENSAGEM IMPORTANTE NO CONSOLE
        Console.WriteLine("**************************************************");
        Console.WriteLine(">>> BACKEND PRONTO!");
        Console.WriteLine($">>> LOGIN: {adminEmail}");
        Console.WriteLine(">>> SENHA: admin123");
        Console.WriteLine(">>> SWAGGER: http://localhost:5000/swagger");
        Console.WriteLine("**************************************************");

    } catch (Exception ex) {
        Console.WriteLine($">>> ERRO CRÍTICO NO SEED: {ex.Message}");
    }
}

app.Run();
