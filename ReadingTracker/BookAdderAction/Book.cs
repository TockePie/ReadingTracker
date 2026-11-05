using System.Text.Json.Serialization;

namespace ReadingTracker.BookAdderAction;

public record Book(
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
    int? Rating,

    [property: JsonPropertyName("currentPage")]
    int CurrentPage = 0
)
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; } = Guid.NewGuid();

    public double? CompletionRate => TotalPages > 0
        ? Math.Round((double)CurrentPage / TotalPages.Value * 100, 1)
        : null;
}