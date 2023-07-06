namespace SwizlyPeasy.Consul.KeyValueStore
{
    public interface IKeyValueService
    {
        public Task SaveToKeyValueStore(string key, byte[] value);
        public Task<byte[]> GetFromKeyValueStore(string key);
    }
}
