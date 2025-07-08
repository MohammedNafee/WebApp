namespace WebAPIApp.Authority
{
    // This class simulates a repository for applications.
    // In a real-world application, this would likely connect to a database or an external service.
    public static class AppRepository
    {
        // Simulated in-memory storage for applications
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
