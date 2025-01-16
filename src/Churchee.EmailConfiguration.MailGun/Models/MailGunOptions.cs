namespace Churchee.EmailConfiguration.MailGun.Models
{
    public sealed class MailGunOptions
    {
        public string APIKey { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string Domain { get; set; }
    }
}
