using MongoDB.Bson;
using MongoDB.Driver;
using DMS.Models;
using DMS.Secrets;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Http.HttpResults;

// TODO: Database for login

namespace DMS.Utils
{
    public static class DBHelper
    {
        private static MongoClient _client;
        private static IMongoDatabase database;

        private static string firmwareDomain = "http://vistimalik.com:8000";
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
        public static async Task<bool> AddToken(string username, string token)
        {
            var collection = database.GetCollection<BsonDocument>("tokens");


            // Create new token user pair in db
            var tokenEntry = new BsonDocument
            {
                { "user", username},
                { "token", token},
                { "time", DateTime.UtcNow}
            };

            // Insert log entry into the database
            try
            {
                collection.InsertOne(tokenEntry);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static async Task<string> Token2ID(string token)
        {
            // Select db
            var collection = database.GetCollection<BsonDocument>("tokens");

            // Create query and execute
            var filter = Builders<BsonDocument>.Filter.Eq("token", token);
            var result = await collection.Find(filter).FirstOrDefaultAsync();


            // Return user if found
            if (result != null)
            {
                return result["user"].AsString;
            }
            else
            {
                return String.Empty;
            }

        }

        public static async Task<List<StatusResponse>> GetStatus(string scaleID, int amount = 100)
        {
            // Select db
            var collection = database.GetCollection<BsonDocument>("scale_logs");

            // Create query and execute
            var filter = Builders<BsonDocument>.Filter.Eq("scaleID", scaleID);

            // Get first {amount} results
            var results = await collection.Find(filter).SortByDescending(doc => doc["_id"]).Limit(amount).ToListAsync();

            // Convert BSONDocument to StatusResponse list
            var statusList = new List<StatusResponse>();

            foreach (var result in results)
            {
                var status = new StatusResponse
                {
                    ScaleID = result["scaleID"].AsString,
                    Version = result["firmwareVersion"].AsString,
                    time = result["time"].ToLocalTime()
                };

                statusList.Add(status);
            }

            return statusList;
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
            try
            {
                collection.InsertOne(scaleLog);
                return true;
            }
            catch
            {
                return false;
            }
        }
        // Add weight to worker_contributions table
        public static async Task<bool> AddWeight(string workerID, int type, float weight, string period, DateTime time)
        {
            var collection = database.GetCollection<BsonDocument>("worker_contributions");

            // Create new log entry
            var weightEntry = new BsonDocument
            {
                { "worker_id", workerID},
                { "material_id", type },
                { "contribution", weight},
                {"period", period},
                {"last_updated", time}
            };


            // Insert log entry into the database
            try
            {
                collection.InsertOne(weightEntry);
                return true;
            }
            catch
            {
                return false;
            }

        }
        
        public static async Task<Dictionary<string, string>> GetNewestVersion()
        {
            // Select db
            var collection = database.GetCollection<BsonDocument>("firmwareVersions");

            // Get first {amount} results
            var results = await collection.Find(new BsonDocument()).SortByDescending(doc => doc["_id"]).Limit(1).ToListAsync();

            // Select latest result
            var result = results[0];


            // Craft response dict of version and path of firmware in firmware store
            var responseDict = new Dictionary<string, string>
            {
                {"version", result.GetValue("version").AsString },
                { "path", result.GetValue("path").AsString}
            };
            return responseDict;
        }
        
        public static async Task<bool> AddFirmware(string version, string fileName)
        {
            // Select db
            var collection = database.GetCollection<BsonDocument>("firmwareVersions");

            // Get current time
            DateTime time = DateTime.Now;

            // Create new log entry
            var firmwareLog = new BsonDocument
            {
                { "version", version},
                { "path", $"http://vistimalik.com:8000/{fileName}" },
                { "time", time }
            };

            // Insert log entry into the database
            try
            {
                collection.InsertOne(firmwareLog);
                return true;
            }
            catch
            {
                return false;
            }
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
                // Generate uuid as token
                string token = Guid.NewGuid().ToString();
                authResponse.Token = token;

                // Add tokenpair to database
                await DBHelper.AddToken(username, token);

                // Update response message to correct response
                authResponse.Message = "Login successful";
            }
            else
            {
                // Update response message to correct response
                authResponse.Message = "Login failed";
            }
            return authResponse;
        }

        // Authenticate with token
        public static async Task<bool> AuthFromToken(string token)
        {
            // Retrive user from token
            string tokenValidity = await DBHelper.Token2ID(token);


            // If no user found then auth failed
            if (tokenValidity == String.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }

    public static class Helpers
    {
        public static async Task<bool> GetUpDownStatus(string scaleID)
        {
            // Get latest status messages of scale 
            var statusList = await DBHelper.GetStatus(scaleID, 1);
            try
            {
                // Choose latest element of list
                var status = statusList[0];
                // If no status message was recived within the last 5 minutes, the scale is down
                if ((DateTime.Now - status.time).TotalMinutes < 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            // No elements found means scale not found
            catch
            {
                return false;
            }
        }

        public static async Task<string> GetScaleVersion(string scaleID)
        {
            // Get latest status messages of scale 
            var statusList = await DBHelper.GetStatus(scaleID, 1);
            try
            {
                // Choose latest element of list
                var status = statusList[0];

                // return version number
                return status.Version;
            }
            catch
            {
                return string.Empty;
            }

        }
    }
}
