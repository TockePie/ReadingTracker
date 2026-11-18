using System.Reflection;
using System.Text;

namespace ReadingTracker.Storage;

public class CsvStorage : IStorage
{
    private static readonly char[] SpecialCharacters = [',', '"', '\r', '\n'];

    public Task<List<T>> Get<T>() where T : IEntity
    {
        throw new NotImplementedException();
    }

    public Task RemoveById<T>(Guid id) where T : IEntity
    {
        throw new NotImplementedException();
    }

    public Task Save<T>(T content) where T : IEntity
    {
        throw new NotImplementedException();
    }

    public Task UpdateItem<T>(Guid id, T content) where T : IEntity
    {
        throw new NotImplementedException();
    }

    private static string BuildCsvString<T>(IEnumerable<T> content) where T : IEntity
    {
        var stringBuilder = new StringBuilder();

        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetIndexParameters().Length == 0)
            .ToArray();

        var keys = properties.Select(obj => obj.Name).ToArray();
        var stringKeys = string.Join(',', keys);

        stringBuilder.AppendLine(EscapeChar(stringKeys));

        foreach (var item in content)
        {
            var values = properties
                .Select(obj => obj.GetValue(item)?.ToString() ?? string.Empty)
                .ToArray();

            var stringValues = string.Join(',', values);
            stringBuilder.AppendLine(EscapeChar(stringValues));
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