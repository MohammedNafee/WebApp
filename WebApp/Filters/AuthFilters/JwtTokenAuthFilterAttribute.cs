using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

            // 4. Verify the token
            if (await Authenticator.VerifyTokenAsync(tokenString, secretKey))
            {
                return;
            }
            else
            {
                context.Result = new UnauthorizedResult();
                return;
            }

        }
    }
}
