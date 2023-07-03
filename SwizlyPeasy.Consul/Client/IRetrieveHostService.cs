namespace SwizlyPeasy.Consul.Client
{
    public interface IRetrieveHostService
    {
        Task<Uri> RetrieveHostFromServiceName(string serviceName);
    }
}