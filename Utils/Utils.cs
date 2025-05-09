using MongoDB.Bson;
using MongoDB.Driver;
using DMS.Models;
using DMS.Secrets;

// TODO: Generate token
// TODO: Auth by token
// TODO: Database for login

namespace DMS.Utils
{
    public static class DBHelper
    {
        private static MongoClient _client;
        private static IMongoDatabase database;

        public static void ConnectToDB()
        {
            // Connection string to MongoDB Atlas
            string connectionUri = "mongodb+srv://" + Secrets.Secrets.DBUsername + ":" + Secrets.Secrets.DBPassword + "@dmscluster1.gao9ej8.mongodb.net/?retryWrites=true&w=majority&appName=DMSCluster1";
            _client = new MongoClient(connectionUri);

            // Set database
            database = _client.GetDatabase("DMS");
        }

        // Login query 
        public static async Task<UserCreds> ReturnLogin(string username, string password)
        {
            // Placeholder 
            var creds = new UserCreds
            {
                Username = "wasteworker",
                Password = "verysecretpassword"
            };

            // Simulate a database query
            await Task.Delay(1000);
            return creds;
        }
        public static async Task<string> Token2ID(string token)
        {
            // Simulate a database query
            await Task.Delay(1000);
            return "PLACEHOLDER_ID";
        }
        // Add status to database
        public static async Task<bool> AddStatus(string scaleID, string firmwareVersion, DateTime time)
        {
            var collection = database.GetCollection<BsonDocument>("scale_logs");

            // Create new log entry
            var scaleLog = new BsonDocument
            {
                { "scaleID", scaleID},
                { "firmwareVersion", firmwareVersion },
                { "time", time }
            };

            // Insert log entry into the database
            collection.InsertOne(scaleLog);

            return true;
        }
    }

    public static class AuthHelper
    {
        // Authenticate with credentials
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

        // Authenticate with token
        public static async Task<bool> AuthFromToken(string token)
        {
            // Simulate token authentication
            await Task.Delay(1000);

            if (token != "PLACEHOLDER_TOKEN")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
