namespace SwizlyPeasy.Common.Dtos;

public class DataProtectionConfig
{
    public string ApplicationName { get; set; } = "SwizlyPeasy.Gateway";
    public string? KeyRingPath { get; set; }
}
