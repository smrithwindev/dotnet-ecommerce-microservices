using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Web.Authorization
{
    public class FeaturePermissionHandler : AuthorizationHandler<FeaturePermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,FeaturePermissionRequirement requirement)
        {
            var permissionClaims = context.User.FindAll("Permissions");
            foreach (var claim in permissionClaims)
            {
                var parts = claim.Value.Split(':');

                if (parts.Length != 2)
                    continue;

                var feature = parts[0];

                if (feature != requirement.Feature)
                    continue;

                if (int.TryParse(parts[1], out int userLevel) &&
                    userLevel >= (int)requirement.RequiredLevel)
                {
                    context.Succeed(requirement);
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
