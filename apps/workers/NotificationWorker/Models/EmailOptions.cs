namespace Waggle.NotificationWorker.Models
{
    public class EmailOptions
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string Username { get; set; } = "waggle.noreply@gmail.com";
        public string Password { get; set; } = "password";
        public string From { get; set; } = "waggle.noreply@gmail.com";
    }
}
