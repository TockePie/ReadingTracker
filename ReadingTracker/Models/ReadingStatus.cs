using System.Text.Json.Serialization;

namespace ReadingTracker.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReadingStatus
{
    WantToRead,
    Reading,
    Completed,
}
