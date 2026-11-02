using System.Text.Json.Serialization;

namespace ReadingTracker.BookAdder;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReadingStatus
{
    WantToRead,
    Reading,
    Completed,
}
