namespace MLPos.Web.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
