using BuildingBlocks.Core.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Web.Authorization
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute(string feature, PermissionLevel level)
        {
            Policy = $"Feature:{feature}:{(int)level}";
        }
    }
}
