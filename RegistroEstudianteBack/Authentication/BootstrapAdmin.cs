using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Users.Abstractions;
using RegistroEstudiante.Domain.Constants;

namespace RegistroEstudianteBack.Authentication;

public static class BootstrapAdmin
{
    public static async Task InitializeAsync(WebApplication app)
    {
        var options = app.Configuration.GetSection("BootstrapAdmin").Get<BootstrapAdminOptions>();
        if (options is null || !options.Enabled) return;

        await using var scope = app.Services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var users = scope.ServiceProvider.GetRequiredService<IUserService>();
        // Nunca cambia credenciales ni promueve una cuenta ya existente.
        if (await repository.HasAdminAsync(CancellationToken.None)) return;
        options.Role = RoleNames.Admin;
        options.Validate();
        await users.CreateAsync(options, CancellationToken.None);
        app.Logger.LogInformation("Administrador inicial creado.");
    }
}
