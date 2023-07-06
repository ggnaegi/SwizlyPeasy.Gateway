using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Gateway.Services
{
    public interface IRoutesConfigService
    {
        public Task LoadRoutes(string key, string sectionString);
        public Task<RouteConfig[]> GetRoutes(string key);
    }
}
