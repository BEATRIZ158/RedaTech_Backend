using System.Text.Json.Serialization;

namespace Redatech.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusCorrection
    {
        InProgress = 1,
        Completed = 2,
        UnderReview = 3
    }
}
