using System.ComponentModel;

namespace ReadingTracker.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum enumValue)
    {
        var enumType = enumValue.GetType();
        var stringValue = enumValue.ToString();
        var field = enumType.GetField(stringValue);

        var finalResult = Attribute.GetCustomAttribute(
            field,
            typeof(DescriptionAttribute)
        );

        if (finalResult is not DescriptionAttribute attribute)
        {
            return stringValue;
        }
        else
        {
            return attribute.Description;
        }
    }
}