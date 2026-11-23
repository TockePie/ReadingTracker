using System.Reflection;
using System.Text;

namespace ReadingTracker.Exporters;

public class CsvExporter : IExporter
{
    public string FormatName => "CSV";

    public string FileExtension => "csv";

    private static readonly char[] SpecialCharacters = [',', '"', '\r', '\n'];

    public async Task ExportAsync<T>(IEnumerable<T> content, string filePath)
    {
        var result = BuildString(content);

        await File.WriteAllTextAsync(filePath, result, new UTF8Encoding(false));
    }

    private static string BuildString<T>(IEnumerable<T> content)
    {
        var stringBuilder = new StringBuilder();

        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetIndexParameters().Length == 0)
            .ToArray();

        var keys = properties.Select(obj => EscapeChar(obj.Name)).ToArray();
        var stringKeys = string.Join(',', keys);

        stringBuilder.AppendLine(stringKeys);

        foreach (var item in content)
        {
            var values = properties
                .Select(obj => EscapeChar(obj.GetValue(item)?.ToString() ?? string.Empty))
                .ToArray();

            var stringValues = string.Join(',', values);
            stringBuilder.AppendLine(stringValues);
        }

        return stringBuilder.ToString();
    }

    private static string EscapeChar(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return string.Empty;
        }

        if (field.IndexOfAny(SpecialCharacters) >= 0)
        {
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }

        return field;
    }
}