using System.Net;
using Consul;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Consul.KeyValueStore
{
    public class KeyValueService: IKeyValueService
    {
        private readonly IConsulClient _consulClient;
        public KeyValueService(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public async Task SaveToKeyValueStore(string key, byte[] value)
        {
            var kvPair = new KVPair(key)
            {
                Value = value
            };

            var writeResult = await _consulClient.KV.Put(kvPair);
            if (!writeResult.Response)
            {
                throw new InternalDomainException("Unable to save data to key value store...");
            }
        }

        public async Task<byte[]> GetFromKeyValueStore(string key)
        {
            var result = await _consulClient.KV.Get(key);

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InternalDomainException($"Key: {key} not found in key value store...");
            }

            return result.Response.Value;
        }
    }
}
