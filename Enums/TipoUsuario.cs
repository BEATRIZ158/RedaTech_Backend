using System.Text.Json.Serialization;

namespace Redatech.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoUsuario
    {
        Aluno = 1,
        Professor = 2,
        Administrador = 3
    }
}
