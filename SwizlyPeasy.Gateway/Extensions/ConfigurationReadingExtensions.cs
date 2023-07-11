using System.Collections.ObjectModel;
using System.Globalization;

namespace SwizlyPeasy.Gateway.Extensions;

/// <summary>
///     I have used part of the configuration reading mechanism implemented by Microsoft
///     https://github.com/microsoft/reverse-proxy/blob/5cad42d60a514f2b93fc47968d4405c6d06d39f2/src/ReverseProxy/Configuration/ConfigProvider/ConfigurationReadingExtensions.cs
/// </summary>
public static class ConfigurationReadingExtensions
{
    internal static int? ReadInt32(this IConfiguration configuration, string name)
    {
        return configuration[name] is { } value
            ? int.Parse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture)
            : null;
    }

    internal static long? ReadInt64(this IConfiguration configuration, string name)
    {
        return configuration[name] is { } value
            ? long.Parse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture)
            : null;
    }

    internal static IReadOnlyDictionary<string, string>? ReadStringDictionary(this IConfigurationSection section)
    {
        if (section.GetChildren() is var children && !children.Any()) return null;

        return new ReadOnlyDictionary<string, string>(children.ToDictionary(s => s.Key, s => s.Value!,
            StringComparer.OrdinalIgnoreCase));
    }

    internal static string[]? ReadStringArray(this IConfigurationSection section)
    {
        if (section.GetChildren() is var children && !children.Any()) return null;

        return children.Select(s => s.Value!).ToArray();
    }

    internal static TEnum? ReadEnum<TEnum>(this IConfiguration configuration, string name) where TEnum : struct
    {
        return configuration[name] is { } value ? Enum.Parse<TEnum>(value, true) : null;
    }

    internal static bool? ReadBool(this IConfiguration configuration, string name)
    {
        return configuration[name] is { } value ? bool.Parse(value) : null;
    }
}