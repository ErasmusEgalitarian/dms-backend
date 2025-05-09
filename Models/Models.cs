namespace DMS.Models {
    public class UserCreds {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class LoginResponse {
        public string Token { get; set; } = string.Empty;    
        public string Message { get; set; } = string.Empty;
        }
}