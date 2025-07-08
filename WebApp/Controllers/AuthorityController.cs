using Microsoft.AspNetCore.Mvc;
using WebAPIApp.Authority;

namespace WebAPIApp.Controllers
{
    [ApiController]
    public class AuthorityController : ControllerBase
    {
        private readonly IConfiguration configuration;
        
        public AuthorityController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] AppCredential credential)
        {
            // Validate client credentials against the registered apps in the repository.
            if (Authenticator.Authenticate(credential.ClientId, credential.ClientSecret))
            {
                var expiresAt = DateTime.UtcNow.AddMinutes(1); // Token is valid for 1 minute
                // If valid, generate an access token and return it with expiration info.
                return Ok(new
                {
                    access_token = Authenticator.CreateToken(credential.ClientId, expiresAt, configuration["SecurityKey"] ?? string.Empty), // Creates a secure token for the client
                    expires_at = expiresAt
                });
            }
            else
            {
                // If credentials are invalid, add an error to the ModelState.
                ModelState.AddModelError("Unauthorized", "The user is not authorized!");

                // Create a standardized error response using ValidationProblemDetails.
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status401Unauthorized
                };

                return new UnauthorizedObjectResult(problemDetails);    
            }
        }
    }
}
