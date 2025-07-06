using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

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

            // The hashing Algorithm + Signing Secret Key
            var signingCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(strSecretKey)),
                    SecurityAlgorithms.HmacSha256Signature
                );

            // Payload(claims)
            var app = AppRepository.GetApplicationByClientId(clientId);

            var claimsDictionary = new Dictionary<string, object>
            {
                {"AppName", app?.ApplicationName??string.Empty},
                {"Read", (app?.Scopes??String.Empty).Contains("read") ? "true" : "false"},
                {"Write", (app?.Scopes??String.Empty).Contains("write") ? "true" : "false"}
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Claims = claimsDictionary,
                Expires = expiresAt,
                NotBefore = DateTime.UtcNow
            };

            var tokenHandler = new JsonWebTokenHandler();

            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public static async Task<bool> VerifyTokenAsync(string token, string secretKey)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(secretKey))
                return false;

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
                return result.IsValid;

            }
            catch (SecurityTokenMalformedException)
            {
                // Token is malformed
                return false;
            }
            catch (SecurityTokenExpiredException)
            {
                // Token has expired
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                // Token signature is invalid
                return false;
            }
            catch (Exception)
            {
                // Other exceptions
                throw;
            }
        }
    }
}
