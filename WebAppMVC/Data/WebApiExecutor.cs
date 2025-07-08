

using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace WebAppMVC.Data
{
    public class WebApiExecutor : IWebApiExecutor
    {
        private const string apiName = "ShirtsApi";
        private const string authApiName = "AuthorityApi";

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public WebApiExecutor(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<T?> InvokeGet<T>(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);

            //return await httpClient.GetFromJsonAsync<T>(relativeUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
            var response = await httpClient.SendAsync(request);
            await HandlePotentialError(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task InvokePost<T>(string relativeUrl, T obj)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);

            var response = await httpClient.PostAsJsonAsync(relativeUrl, obj);

            await HandlePotentialError(response);

            //return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task InvokePut<T>(string relativeUrl, T obj)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);

            var response = await httpClient.PutAsJsonAsync(relativeUrl, obj);

            await HandlePotentialError(response);
        }

        public async Task InvokeDelete(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);

            var response = await httpClient.DeleteAsync(relativeUrl);

            await HandlePotentialError(response);
        }

        private async Task HandlePotentialError(HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorJson = await httpResponse.Content.ReadAsStringAsync();

                throw new WebApiException(errorJson);
            }
        }

        private async Task AddJwtToHeader(HttpClient httpClient)
        {
            JwtToken? jwtToken = null;

            string? strToken = httpContextAccessor?.HttpContext?.Session.GetString("access_token");

            if (!string.IsNullOrWhiteSpace(strToken))
            {
                // If the token is already in the session, deserialize it
                jwtToken = JsonConvert.DeserializeObject<JwtToken>(strToken);
            }

            if (string.IsNullOrWhiteSpace(strToken))
            {
                // 1. Authenticate against the Authority API to get a JWT token
                // For Authontication, we will use the AppCredential class
                // which contains ClientId and ClientSecret to authenticate against the Authority API

                var clientId = configuration.GetValue<string>("ClientId");
                var clientSecret = configuration.GetValue<string>("ClientSecret");

                // We will use the IHttpClientFactory to create a client to communicate with the Authority API
                var authoClient = httpClientFactory.CreateClient(authApiName);

                // Post the AppCredential to the Authority API to get a JWT token
                var response = await authoClient.PostAsJsonAsync("auth", new AppCredential
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                });

                response.EnsureSuccessStatusCode();

                // 2. Get the JWT token from the authority API response
                // For getting the JWT token, we will need a classs that represents the structure of the JWT token
                // Then we can deserialize the response to that class

                strToken = await response.Content.ReadAsStringAsync();
                jwtToken = JsonConvert.DeserializeObject<JwtToken>(strToken);

                // Store the JWT token in the session for later use
                httpContextAccessor?.HttpContext?.Session.SetString("access_token", strToken);

            }

            // 3. Pass the JWT token to endpoints through the http headers
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken?.AccessToken);
        }

    }
}
