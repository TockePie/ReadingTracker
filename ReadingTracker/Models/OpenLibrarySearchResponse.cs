using System.Text.Json.Serialization;

namespace ReadingTracker.Models;

public record OpenLibrarySearchResponse(
    [property: JsonPropertyName("start")]
    int Start,

    [property: JsonPropertyName("num_found")]
    int NumFound,

    [property: JsonPropertyName("docs")]
    IReadOnlyList<ExternalBook> Docs
);

public record ExternalBook(
    [property: JsonPropertyName("author_name")]
    IReadOnlyList<string>? AuthorName,

    [property: JsonPropertyName("cover_i")]
    int? CoverId,

    [property: JsonPropertyName("first_publish_year")]
    int? FirstPublishYear,

    [property: JsonPropertyName("key")]
    string? OpenLibraryKey,

    [property: JsonPropertyName("language")]
    IReadOnlyList<string>? Language,

    [property: JsonPropertyName("title")]
    string Title,

    [property: JsonPropertyName("subtitle")]
    string? Subtitle
);
