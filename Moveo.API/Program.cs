using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios; // Aquí vive SnsService
using Moveo.AccesoDatos.Repositorios;
using System.Text;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Moveo.API.Middleware;
using Moveo.Modelos.Configuraciones;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Configuración de Cloudinary
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);

// Servicios base
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Moveo API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"{token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// --- CONFIGURACIÓN DE CORS ACTUALIZADA ---
// Permitimos localhost para desarrollo y cualquier origen para el despliegue en K8s/Móvil
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMoveoApps", policy =>
    {
        policy.AllowAnyOrigin() // En K8s es mejor AllowAnyOrigin para evitar bloqueos iniciales
              .AllowAnyHeader()
              .AllowAnyMethod();
        // .AllowCredentials(); // Nota: AllowCredentials no funciona con AllowAnyOrigin. 
        // Si usas cookies, deberás poner las URLs específicas aquí.
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["X-Access-Token"];
                return Task.CompletedTask;
            }
        };
    });

// --- INYECCIÓN DE DEPENDENCIAS ---
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUploadService, CloudinaryUploadService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVehiculoRepository, VehiculoRepository>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();
builder.Services.AddScoped<IMantenimientoRepository, MantenimientoRepository>();
builder.Services.AddScoped<IMantenimientoService, MantenimientoService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IEntregaRepository, EntregaRepository>();
builder.Services.AddScoped<IEntregaService, EntregaService>();
builder.Services.AddScoped<IEstadisticaRepository, EstadisticaRepository>();
builder.Services.AddScoped<IEstadisticaService, EstadisticaService>();
builder.Services.AddScoped<IEstadoSesionRepository, EstadoSesionRepository>();
builder.Services.AddScoped<IEstadoSesionService, EstadoSesionService>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IRutaRepository, RutaRepository>();
builder.Services.AddScoped<IRutaService, RutaService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IDistanciasService, DistanciasService>();
builder.Services.AddScoped<IClimaService, ClimaService>();
builder.Services.AddScoped<IIaOptimizationService, IaOptimizationService>();

// REGISTRO DEL NUEVO SERVICIO DE AWS SNS
builder.Services.AddScoped<SnsService>();

var app = builder.Build();

// Pipeline de peticiones
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activamos la política de CORS
app.UseCors("AllowMoveoApps");

app.UseAuthentication();
app.UseAuthorization();

app.UseUserActivity();

app.MapControllers();

app.Run();

public partial class Program { }