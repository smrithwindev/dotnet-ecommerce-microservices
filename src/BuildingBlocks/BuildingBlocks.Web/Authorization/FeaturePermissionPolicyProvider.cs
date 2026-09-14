using BuildingBlocks.Core.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Web.Authorization
{
    public class FeaturePermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public FeaturePermissionPolicyProvider(
            IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider =
                new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith("Feature:"))
            {
                return _fallbackPolicyProvider.GetPolicyAsync(policyName);
            }

            var parts = policyName.Split(':');

            if (parts.Length != 3)
            {
                return Task.FromResult<AuthorizationPolicy?>(null);
            }

            var feature = parts[1];

            if (!int.TryParse(parts[2], out var level))
            {
                return Task.FromResult<AuthorizationPolicy?>(null);
            }

            var requiredLevel = (PermissionLevel)level;

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(
                    new FeaturePermissionRequirement(
                        feature,
                        requiredLevel))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}
