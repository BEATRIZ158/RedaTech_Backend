using System.Text.Json.Serialization;

namespace Redatech.Enums
{
   
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusCorrecao
    {
        EmAndamento = 1,
        Concluida = 2,
        Revisao = 3
    }
}
