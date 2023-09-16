namespace SwizlyPeasy.Common.Dtos;

public class ProxiesAndNetworksConfig
{
    public bool UseForwardedHeaders { get; set; } = false;
    public string[] Proxies { get; set; } = null!;
    public string[] KnownNetworks { get; set; } = null!;
    public int ForwardLimit { get; set; } = 1;
}