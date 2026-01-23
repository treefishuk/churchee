namespace Churchee.EmailConfiguration.MailGun.Models
{
    public sealed class MailGunOptions
    {
        public string APIKey { get; set; }

        public string FromName { get; set; }

        public string FromEmail { get; set; }

        public string Domain { get; set; }

        /// <summary>
        /// Could be either od the following:
        /// <list type="number">
        /// <item>https://api.mailgun.net/v3/</item>
        /// <item>https://api.eu.mailgun.net/v3/</item>
        /// </list>
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
