namespace RegistroEstudiante.Infrastructure.Configuration;

public static class LocalEnvironment
{
    public static void Load()
    {
        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true") return;
        var path = FindFile(Directory.GetCurrentDirectory()) ?? FindFile(AppContext.BaseDirectory);
        if (path is null) return;
        Apply(ReadFile(path), Environment.GetEnvironmentVariable, Environment.SetEnvironmentVariable);
    }

    public static string? FindFile(string startDirectory)
    {
        for (var directory = new DirectoryInfo(startDirectory); directory is not null; directory = directory.Parent)
        {
            if (!File.Exists(Path.Combine(directory.FullName, "RegistroEstudianteBack.slnx"))) continue;
            var path = Path.Combine(directory.FullName, ".env");
            return File.Exists(path) ? path : null;
        }
        return null;
    }

    // Single-line literal values; SQL strings should use outer single quotes.
    public static Dictionary<string, string> ReadFile(string path)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var lineNumber = 0;
        foreach (var raw in File.ReadLines(path))
        {
            lineNumber++;
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            var separator = line.IndexOf('=');
            if (separator <= 0) throw new FormatException($"Formato inválido en .env, línea {lineNumber}.");
            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();
            if (value.StartsWith('\'') || value.StartsWith('"'))
            {
                var end = value.LastIndexOf(value[0]);
                if (end == 0) throw new FormatException($"Comillas inválidas en .env, línea {lineNumber}.");
                var tail = value[(end + 1)..].Trim();
                if (tail.Length > 0 && !tail.StartsWith('#'))
                    throw new FormatException($"Formato inválido en .env, línea {lineNumber}.");
                value = value[1..end];
            }
            else
            {
                var comment = value.IndexOf(" #", StringComparison.Ordinal);
                if (comment >= 0) value = value[..comment].TrimEnd();
            }
            values[key] = value;
        }
        return values;
    }

    public static void Apply(Dictionary<string, string> values, Func<string, string?> get, Action<string, string?> set)
    {
        foreach (var (key, value) in values)
            if (get(key) is null) set(key, value);

        if (get("Jwt__SigningKey") is null && get("JWT_SIGNING_KEY") is string jwt)
            set("Jwt__SigningKey", jwt);
    }
}
