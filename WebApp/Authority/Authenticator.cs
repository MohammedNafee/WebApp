using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace WebAPIApp.Authority
{
    public static class Authenticator
    {
        public static bool Authenticate(string clientId, string clientSecret)
        {
            var app = AppRepository.GetApplicationByClientId(clientId);

            if (app == null)
                return false;

            return (app.ClientSecret == clientSecret && app.ClientId == clientId);
        }

        public static string CreateToken(string clientId, DateTime expiresAt, string strSecretKey)
        {
            // Hashing Algorithm
            // Signing Secret Key

            // Payload (claims => which contains the application information)

            // Signing Credentials combine = The hashing Algorithm + Signing Secret Key
            var signingCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(strSecretKey)),
                    SecurityAlgorithms.HmacSha256Signature
                );

            // Payload(claims)
            var app = AppRepository.GetApplicationByClientId(clientId);

            var claimsDictionary = new Dictionary<string, object>
            {
                {"AppName", app?.ApplicationName??string.Empty}
            };

            var scopes = app?.Scopes?.Split(',') ?? Array.Empty<string>();
            
            if (scopes.Length > 0)
            {
                foreach (var scope in scopes)
                {
                    claimsDictionary.Add(scope.Trim().ToLower(), "true");
                }
            }

            // wrapping the claims in a ClaimsIdentity 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Claims = claimsDictionary,
                Expires = expiresAt,
                NotBefore = DateTime.UtcNow
            };

            var tokenHandler = new JsonWebTokenHandler();

            // Create the token using the token handler and the token descriptor
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public static async Task<IEnumerable<Claim>?> VerifyTokenAsync(string token, string secretKey)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(secretKey))
                return null;

            var keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            var tokenHandler = new JsonWebTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No clock skew
            };

            try
            {
                var result = await tokenHandler.ValidateTokenAsync(token, validationParameters);

                if (result.SecurityToken != null)
                {
                    var tokenObject = tokenHandler.ReadJsonWebToken(token);
                    return tokenObject.Claims ?? Enumerable.Empty<Claim>();
                }
                else
                {
                    // Token is not valid
                    return null;
                }

            }
            catch (SecurityTokenMalformedException)
            {
                // Token is malformed
                return null;
            }
            catch (SecurityTokenExpiredException)
            {
                // Token has expired
                return null;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                // Token signature is invalid
                return null;
            }
            catch (Exception)
            {
                // Other exceptions
                throw;
            }
        }
    }
}
