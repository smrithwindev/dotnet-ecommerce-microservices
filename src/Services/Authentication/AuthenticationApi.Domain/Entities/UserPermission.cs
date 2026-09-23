using BuildingBlocks.Core.Authorization;

namespace AuthenticationApi.Domain.Entities
{
    public class UserPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Feature { get; set; } = string.Empty;
        public PermissionLevel PermissionLevel { get; set; }
        public AppUser User { get; set; } = null!;
    }
}
