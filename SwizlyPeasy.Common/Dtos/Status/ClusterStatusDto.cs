namespace SwizlyPeasy.Common.Dtos.Status;

public class ClusterStatusDto
{
    public string? ClusterId { get; set; }
    public bool Healthy { get; set; }
    public IList<DestinationStatusDto>? Destinations { get; set; }
}