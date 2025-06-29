namespace WebAPIApp.Authority
{
    public static class AppRepository
    {
        private static List<Application> _applications = new List<Application>()
        {
            new Application
            {
                ApplicationId = 1, 
                ApplicationName = "MVCWebApp", 
                ClientId = "123", 
                ClientSecret = "12345"
            }
        };

        public static bool Authenticate(string clientId,  string clientSecret)
        {
            return _applications.Any(app => app.ClientId == clientId && app.ClientSecret == clientSecret);
        }

        public static Application? ApplicationByClientId(string clientId)
        {
            return _applications.FirstOrDefault(app => app.ClientId == clientId);
        }
    }
}
