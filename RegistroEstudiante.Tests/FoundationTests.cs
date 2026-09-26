using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using RegistroEstudiante.Application;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Roles.Abstractions;
using RegistroEstudiante.Application.Subjects.Abstractions;
using RegistroEstudiante.Application.Users.Abstractions;
using RegistroEstudiante.Domain.Entities;
using RegistroEstudiante.Infrastructure;
using RegistroEstudiante.Infrastructure.Persistance;
using Xunit;

namespace RegistroEstudiante.Tests;

public class FoundationTests
{
    private static ApplicationDbContext CreateContext()
        => new(
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=localhost;Database=ModelTests;Integrated Security=true;TrustServerCertificate=true")
            .AddInterceptors(new SuppressDatabaseWrites())
            .Options);

    [Fact]
    public void StudentsCanShareSubjectsThroughJoinTable()
    {
        using var db = CreateContext();
        var enrollment = db.Model.FindEntityType(typeof(StudentSubject))!;
        Assert.Equal("StudentSubjects", enrollment.GetTableName());
        Assert.Equal(new[] { nameof(StudentSubject.SubjectId), nameof(StudentSubject.UserId) },
            enrollment.FindPrimaryKey()!.Properties.Select(x => x.Name));
        var relationships = enrollment.GetForeignKeys().ToList();
        Assert.Equal(2, relationships.Count);
        Assert.Contains(relationships, x => x.PrincipalEntityType.ClrType == typeof(User) && !x.IsUnique);
        Assert.Contains(relationships, x => x.PrincipalEntityType.ClrType == typeof(Subject) && !x.IsUnique);
        Assert.NotNull(db.Model.FindEntityType(typeof(User))!.FindNavigation(nameof(User.StudentSubjects)));
        Assert.NotNull(db.Model.FindEntityType(typeof(Subject))!.FindNavigation(nameof(Subject.StudentSubjects)));
    }

    [Fact]
    public void AutoMapperConfigurationAndEnrollmentMappingsAreValid()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<AutoMapper.IMapper>();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        var subjectEnrollment = mapper.Map<StudentSubject>(new Subject { Id = 15 });
        Assert.Equal(15, subjectEnrollment.SubjectId);
        Assert.Equal(default, subjectEnrollment.CreationDate);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SaveChangesSetsUtcAuditAndPreservesCreationDate(bool asynchronous)
    {
        using var db = CreateContext();
        var student = new User { Name = "Ana" };
        db.Users.Add(student);
        var before = DateTime.UtcNow;
        if (asynchronous) await db.SaveChangesAsync(); else db.SaveChanges();
        Assert.InRange(student.CreationDate, before, DateTime.UtcNow);
        Assert.Equal(DateTimeKind.Utc, student.CreationDate.Kind);
        Assert.Null(student.UpdatedDate);

        // The interceptor skips persistence; mark the insert as accepted explicitly.
        db.ChangeTracker.AcceptAllChanges();
        var created = student.CreationDate;
        student.Name = "Ana Maria";
        if (asynchronous) await db.SaveChangesAsync(); else db.SaveChanges();
        Assert.Equal(created, student.CreationDate);
        Assert.NotNull(student.UpdatedDate);
        Assert.True(student.UpdatedDate >= created);
        Assert.False(db.Entry(student).Property(x => x.CreationDate).IsModified);
    }

    [Fact]
    public void AllControllerServicesAndRepositoriesResolve()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure("Server=localhost;Database=ModelTests;Integrated Security=true");
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
        using var scope = provider.CreateScope();
        foreach (var type in new[] { typeof(IUserService), typeof(IRoleService), typeof(ISubjectService),
                     typeof(IUserRepository), typeof(IRoleRepository), typeof(ISubjectRepository), typeof(ApplicationDbContext) })
            Assert.NotNull(scope.ServiceProvider.GetRequiredService(type));
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 101)]
    [InlineData(int.MaxValue, 100)]
    public void PaginationRejectsInvalidAndOverflowingOffsets(int page, int size)
    {
        Assert.Throws<ValidationException>(() => new Pagination { PageNumber = page, PageSize = size }.Validate());
    }

    private sealed class SuppressDatabaseWrites : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
            => InterceptionResult<int>.SuppressWithResult(0);

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(InterceptionResult<int>.SuppressWithResult(0));
    }
}
