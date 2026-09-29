using RegistroEstudiante.Infrastructure.Configuration;
using Xunit;

namespace RegistroEstudiante.Tests;

public class LocalEnvironmentTests
{
    [Fact]
    public void ReadsConnectionWithoutLosingPasswordCharacters()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "# comentario\nConnectionStrings__DefaultConnection='Server=localhost,14335;Password=\"a$#=b\";'\nJWT_SIGNING_KEY=test # comentario\n");
            var values = LocalEnvironment.ReadFile(path);
            Assert.Equal("Server=localhost,14335;Password=\"a$#=b\";", values["ConnectionStrings__DefaultConnection"]);
            Assert.Equal("test", values["JWT_SIGNING_KEY"]);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void ProcessOverridesWinAndJwtAliasIsLoaded()
    {
        var process = new Dictionary<string, string?> { ["ConnectionStrings__DefaultConnection"] = "process" };
        var file = new Dictionary<string, string> { ["ConnectionStrings__DefaultConnection"] = "file", ["JWT_SIGNING_KEY"] = "key" };
        LocalEnvironment.Apply(file, key => process.GetValueOrDefault(key), (key, value) => process[key] = value);
        Assert.Equal("process", process["ConnectionStrings__DefaultConnection"]);
        Assert.Equal("key", process["Jwt__SigningKey"]);
        process["Jwt__SigningKey"] = "override";
        LocalEnvironment.Apply(file, key => process.GetValueOrDefault(key), (key, value) => process[key] = value);
        Assert.Equal("override", process["Jwt__SigningKey"]);
    }

    [Fact]
    public void FindsFileFromSolutionProjectAndBinaryDirectories()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var project = Path.Combine(root, "RegistroEstudianteBack");
            var binaries = Path.Combine(project, "bin", "Debug", "net10.0");
            Directory.CreateDirectory(binaries);
            File.WriteAllText(Path.Combine(root, "RegistroEstudianteBack.slnx"), "");
            var path = Path.Combine(root, ".env");
            File.WriteAllText(path, "");
            Assert.Equal(path, LocalEnvironment.FindFile(root));
            Assert.Equal(path, LocalEnvironment.FindFile(project));
            Assert.Equal(path, LocalEnvironment.FindFile(binaries));
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void InvalidFileDoesNotExposeItsContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "PASSWORD='private-password");
            var error = Assert.Throws<FormatException>(() => LocalEnvironment.ReadFile(path));
            Assert.DoesNotContain("private-password", error.Message);
        }
        finally { File.Delete(path); }
    }
}
