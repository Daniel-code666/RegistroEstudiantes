using Microsoft.EntityFrameworkCore;
using RegistroEstudiante.Application;
using RegistroEstudiante.Infrastructure;
using System.Reflection;
using RegistroEstudiante.Infrastructure.Persistance;
using RegistroEstudianteBack.Health;
using RegistroEstudianteBack.Middleware;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using RegistroEstudianteBack.Authentication;

if (args.Contains("--healthcheck"))
{
    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(4) };
    try
    {
        var response = await client.GetAsync("http://localhost:8080/health");
        Environment.ExitCode = response.IsSuccessStatusCode ? 0 : 1;
    }
    catch { Environment.ExitCode = 1; }
    return;
}

RegistroEstudiante.Infrastructure.Configuration.LocalEnvironment.Load();
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Registro de estudiantes API",
        Version = "v1",
        Description = "API para el registro de estudiantes. Fechas en UTC."
    });
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Ingrese el accessToken obtenido en POST /api/auth/login."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddApplication(builder.Configuration["AutoMapper:LicenseKey"]);
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection."));
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:InitializeOnStartup"))
{
    app.Logger.LogInformation("Aplicando migraciones de la base de datos.");
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!db.Database.GetMigrations().Any())
        throw new InvalidOperationException("No hay migraciones. Cree la migración inicial antes de habilitar Database:InitializeOnStartup.");
    await db.Database.MigrateAsync();
    app.Logger.LogInformation("Base de datos lista. Iniciando la API.");
}

await BootstrapAdmin.InitializeAsync(app);

app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePages();
app.UseSwagger();

app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Registro Estudiante API v1"));

// Configure the HTTP request pipeline.

if (app.Configuration.GetValue("Http:UseHttpsRedirection", true))
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();
app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous().ExcludeFromDescription();
app.Run();
