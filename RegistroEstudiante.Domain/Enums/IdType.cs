using System.Text.Json.Serialization;

namespace RegistroEstudiante.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoIdentificacion : short
    {
        CedulaCiudadania = 1,
        Nit = 2,
        CedulaExtranjeria = 3,
        Pasaporte = 4,
        TarjetaIdentidad = 5
    }
}
