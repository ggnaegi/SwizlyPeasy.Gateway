using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Gateway.Services
{
    public interface IClusterConfigService
    {
        Task<List<ClusterConfig>> RetrieveClustersConfig();
    }
}
