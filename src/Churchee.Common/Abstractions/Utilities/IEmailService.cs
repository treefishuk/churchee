using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Utilities
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string toName, string subject, string htmlMessage, string plainTextMessage);
    }
}
