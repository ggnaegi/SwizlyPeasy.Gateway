// ReSharper disable InconsistentNaming

namespace SwizlyPeasy.Common.Dtos.Status;

public class Check
{
    public string? Node { get; set; }
    public string? CheckID { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public string? Output { get; set; }
    public string? ServiceID { get; set; }
    public string? ServiceName { get; set; }
    public object? ServiceTags { get; set; }
    public string? Type { get; set; }
    public int ExposedPort { get; set; }
    public Definition? Definition { get; set; }
    public int CreateIndex { get; set; }
    public int ModifyIndex { get; set; }
}

public class Definition
{
    public string? Interval { get; set; }
    public string? Timeout { get; set; }
    public string? DeregisterCriticalServiceAfter { get; set; }
    public string? HTTP { get; set; }
    public object? Header { get; set; }
    public string? Method { get; set; }
    public string? Body { get; set; }
    public string? TLSServerName { get; set; }
    public bool TLSSkipVerify { get; set; }
    public string? TCP { get; set; }
    public string? UDP { get; set; }
    public string? GRPC { get; set; }
    public string? OSService { get; set; }
    public bool GRPCUseTLS { get; set; }
}

public class Meta
{
}

public class HealthEndpointStatusDto
{
    public string? AggregatedStatus { get; set; }
    public ServiceData? Service { get; set; }
    public List<Check>? Checks { get; set; }
}

public class ServiceData
{
    public string? ID { get; set; }
    public string? Service { get; set; }
    public List<object>? Tags { get; set; }
    public Meta? Meta { get; set; }
    public int Port { get; set; }
    public string? Address { get; set; }
    public Weights? Weights { get; set; }
    public bool EnableTagOverride { get; set; }
    public string? Datacenter { get; set; }
}

public class Weights
{
    public int Passing { get; set; }
    public int Warning { get; set; }
}