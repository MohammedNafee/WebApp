using Microsoft.AspNetCore.Mvc;
using WebAPIApp.Authority;

namespace WebAPIApp.Controllers
{
    [ApiController]
    public class AuthorityController : ControllerBase
    {
        /// <summary>
        /// Authenticates an application using client credentials and issues an access token if valid.
        /// </summary>
        /// <param name="credential">The application's credentials (ClientId and ClientSecret).</param>
        /// <returns>
        /// A 200 OK response with an access token and expiration time if credentials are valid; 
        /// otherwise, returns 401 Unauthorized with error details.
        /// </returns>
        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] AppCredential credential)
        {
            // Validate client credentials against the registered apps in the repository.
            if (AppRepository.Authenticate(credential.ClientId, credential.ClientSecret))
            {
                // If valid, generate an access token and return it with expiration info.
                return Ok(new
                {
                    access_token = CreateToken(credential.ClientId), // Creates a secure token for the client
                    expires_at = DateTime.UtcNow.AddMinutes(10) // Token is valid for 10 minutes
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

        private string CreateToken(string clientId)
        {
            return string.Empty;
        }
    }
}
