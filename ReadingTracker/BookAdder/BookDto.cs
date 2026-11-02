using System.Text.Json.Serialization;

namespace ReadingTracker.BookAdder;

public record BookDto(
    [property: JsonPropertyName("title")]
    string Title,

    [property: JsonPropertyName("author")]
    string Author,

    [property: JsonPropertyName("publicationYear")]
    int? PublicationYear,

    [property: JsonPropertyName("totalPages")]
    int? TotalPages,

    [property: JsonPropertyName("status")]
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    ReadingStatus Status,

    [property: JsonPropertyName("rating")]
    int? Rating
);
