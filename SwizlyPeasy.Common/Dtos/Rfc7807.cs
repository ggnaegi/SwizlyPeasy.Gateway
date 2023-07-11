namespace SwizlyPeasy.Common.Dtos;

/// <summary>
///     RFC 7807 standard properties
/// </summary>
public class Rfc7807
{
    public string? Type { get; set; }

    public string? Title { get; set; }

    public int Status { get; set; }

    public string? Detail { get; set; }

    public string? Instance { get; set; }

    public object? Context { get; set; }
}