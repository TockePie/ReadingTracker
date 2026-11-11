using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ReadingTracker.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReadingStatus
{
    [Description("Want to read")]
    WantToRead,

    Reading,
    Completed,
}
