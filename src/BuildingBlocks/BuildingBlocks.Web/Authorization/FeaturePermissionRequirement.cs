using BuildingBlocks.Core.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Web.Authorization
{
    public class FeaturePermissionRequirement : IAuthorizationRequirement
    {
        public string Feature { get; }

        public PermissionLevel RequiredLevel { get; }

        public FeaturePermissionRequirement(
            string feature,
            PermissionLevel requiredLevel)
        {
            Feature = feature;
            RequiredLevel = requiredLevel;
        }
    }
}
