namespace DMS.Models
{
    public class DefaultResponse
    {
        public string Message { get; set; } = string.Empty;
    }
    public class UserCreds
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    public class Status 
    {
        public string Version { get; set; } = string.Empty;
    }
    public class WeightReq
    {
        public float Weight { get; set; } = 0.0f;
        public int Type { get; set; } = 0;
        public string WorkerID { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
    }
    public class StatusResponse
    {
        public string ScaleID { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime time { get; set; } = DateTime.Now;
    }
}