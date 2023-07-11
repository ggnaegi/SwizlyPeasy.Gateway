using SwizlyPeasy.Common.Dtos.Status;

namespace SwizlyPeasy.Gateway.Services
{
    public interface IStatusService
    {
        public Task<StatusDto> GetGatewayStatus();
    }
}
