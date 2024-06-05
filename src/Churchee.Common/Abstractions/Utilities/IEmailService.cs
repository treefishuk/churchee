using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Utilities
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
