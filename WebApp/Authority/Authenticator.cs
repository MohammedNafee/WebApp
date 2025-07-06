using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

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
    }
}
