

using Microsoft.Extensions.DependencyInjection;
using RegistroEstudiante.Application.Enrollments.Abstractions;
using RegistroEstudiante.Application.Enrollments.Repositories;
using RegistroEstudiante.Application.Authentication.Abstractions;
using RegistroEstudiante.Application.Authentication.Repositories;
using RegistroEstudiante.Application.Common.Security;
using RegistroEstudiante.Application.Mapping;
using RegistroEstudiante.Application.Users.Abstractions;
using RegistroEstudiante.Application.Users.Repositories;
using RegistroEstudiante.Application.Roles.Abstractions;
using RegistroEstudiante.Application.Roles.Repositories;
using RegistroEstudiante.Application.Subjects.Abstractions;
using RegistroEstudiante.Application.Subjects.Repositories;

namespace RegistroEstudiante.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, string? autoMapperLicenseKey = null)
        {
            services.AddAutoMapper(config =>
            {
                if (!string.IsNullOrWhiteSpace(autoMapperLicenseKey)) config.LicenseKey = autoMapperLicenseKey;
                config.AddProfile<ApplicationAppProfile>();
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<JwtTokenGenerator>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            return services;
        }
    }
}
