using DMS.Models;

// TODO: Generate token
// TODO: Auth by token
// TODO: Actually connect to the database


namespace DMS.Utils
{
    public static class DBHelper
    {
        public static void ConnectToDB()
        {
            // Simulate a database connection
            Console.WriteLine("Connected to the database.");
        }

        // Retrive data from the database with query
        private static async Task<string> DoQuery()
        {
            await Task.Delay(10000);
            return "From Utils";
        }

        // Login query 
        public static async Task<UserCreds> ReturnLogin(string username, string password)
        {
            // Do query to get user credentials

            // Placeholder 
            var creds = new UserCreds {
                Username = "wasteworker", 
                Password =  "verysecretpassword"
            };

            // Simulate a database query
            await Task.Delay(1000);
            return creds;
        }
    }

    public static class AuthHelper
    {
        public static async Task<LoginResponse> AuthFromCreds(string username, string password)
        {
            // Create response object
            var authResponse = new LoginResponse();

            // Auth with database
            var creds = await DBHelper.ReturnLogin(username, password);
            if (username == creds.Username && password == creds.Password)
            {
                // Simulate token generation
                authResponse.Token = "PLACEHOLDER_TOKEN";
                authResponse.Message = "Login successful";
            }
            else
            {
                authResponse.Message = "Login failed";
            }
            return authResponse;
        }
        public static async<LoginResponse> AuthFromToken()
        {
            var authResponse = new LoginResponse();

            // Simulate token authentication
            await Task.Delay(1000);
            authResponse.Message == "Login successful";

            return authResponse;
        }
    }
}
