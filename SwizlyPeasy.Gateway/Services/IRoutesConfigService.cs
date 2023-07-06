using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Gateway.Services
{
    public interface IRoutesConfigService
    {
        /// <summary>
        /// Saving routes to the the KV Store
        /// -> routes.config.json
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sectionString"></param>
        /// <returns></returns>
        public Task LoadRoutes(string key, string sectionString);

        /// <summary>
        /// Retrieving Routes as RouteConfig array
        /// from consul KV store. The raw bytes retrieved from consul are converted
        /// to a RouteConfig array. Using some methods provided by Microsoft
        /// https://github.com/microsoft/reverse-proxy/blob/5cad42d60a514f2b93fc47968d4405c6d06d39f2/src/ReverseProxy/Configuration/ConfigProvider/ConfigurationConfigProvider.cs
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<RouteConfig[]> GetRoutes(string key);
    }
}
