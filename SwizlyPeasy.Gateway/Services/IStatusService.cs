using SwizlyPeasy.Common.Dtos.Status;

namespace SwizlyPeasy.Gateway.Services;

public interface IStatusService
{
    /// <summary>
    ///     Method retrieving clusters
    ///     statuses
    /// </summary>
    /// <returns>An object summarizing clusters health</returns>
    public Task<StatusDto> GetGatewayStatus();
}