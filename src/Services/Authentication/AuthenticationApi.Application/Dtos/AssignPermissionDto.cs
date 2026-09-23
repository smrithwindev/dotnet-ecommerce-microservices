using BuildingBlocks.Core.Authorization;

namespace AuthenticationApi.Application.Dtos
{
    public record AssignPermissionDto(
    int UserId,
    string Feature,
    PermissionLevel PermissionLevel);

}
