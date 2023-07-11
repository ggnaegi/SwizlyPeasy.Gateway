namespace SwizlyPeasy.Common.Dtos.Status;

public class StatusDto
{
    public string? ServiceDiscoveryAddress { get; set; }
    public DateTime StatusCheckedAt => DateTime.Now;
    public IList<ClusterStatusDto>? Clusters { get; set; }
}