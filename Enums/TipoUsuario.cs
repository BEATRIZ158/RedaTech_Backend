using System.Text.Json.Serialization;

namespace Redatech.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoUsuario
    {
        Aluno = 0,
        Professor = 1
    }
}
