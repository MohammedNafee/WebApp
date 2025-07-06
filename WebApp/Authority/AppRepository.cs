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
                ClientSecret = "12345",
                Scopes = "read,write"
            }
        };

        public static Application? GetApplicationByClientId(string clientId)
        {
            return _applications.FirstOrDefault(app => app.ClientId == clientId);
        }
    }
}
