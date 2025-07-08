using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIApp.Attributes;
using WebAPIApp.Authority;

namespace WebAPIApp.Filters.AuthFilters
{
    public class JwtTokenAuthFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 1. Get Authorization header from the http request
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string tokenString = authorizationHeader.ToString();

            // 2. Get rid of the Brearer prefix
            if (!tokenString.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            else
            {
                tokenString = tokenString.Substring("Bearer ".Length).Trim();
            }

            // 3. Get Configuration and the Secret key
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var secretKey = configuration?["SecurityKey"]??string.Empty;

            // 4. Verify the token and extract claims

            var claims = await Authenticator.VerifyTokenAsync(tokenString, secretKey);
            
            if (claims == null)
            {
                context.Result = new UnauthorizedResult(); // 401 Unauthorized
            }
            else
            {
                // get the claims required for authorization
                var requiredClaims = context.ActionDescriptor.EndpointMetadata
                    .OfType<RequiredClaimAttribute>()
                    .ToList();

                // 403
                if (requiredClaims != null && !requiredClaims.All(rc => claims.Any(c => 
                    c.Type.ToLower() == rc.ClaimType.ToLower() &&
                    c.Value.ToLower() == rc.ClaimValue.ToLower())))
                {
                    context.Result = new StatusCodeResult(403); // 403 Forbidden
                }
            }
        }
    }
}
