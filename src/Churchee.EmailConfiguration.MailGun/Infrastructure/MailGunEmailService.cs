using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Exceptions;
using Churchee.EmailConfiguration.MailGun.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.EmailConfiguration.MailGun.Infrastructure
{
    public class MailGunEmailService : IEmailService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        private readonly HttpClient _httpClient;


        public MailGunEmailService(ILogger<MailGunEmailService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("MailGun");
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlMessage, string plainTextMessage)
        {
            try
            {
                var options = _configuration.GetSection("MailGunOptions").Get<MailGunOptions>() ?? throw new MissingConfirgurationSettingException("MailGunOptions Not Found");

                using MultipartFormDataContent form = [];

                void SetFormParam(string key, string value) => form.Add(new StringContent(value, Encoding.UTF8, MediaTypeNames.Text.Plain), key);

                SetFormParam("from", $"{options.FromName} <{options.FromEmail}>");
                SetFormParam("to", toEmail);
                SetFormParam("subject", subject);
                SetFormParam("text", plainTextMessage);
                SetFormParam("html", htmlMessage);

                var result = await _httpClient.PostAsync(string.Empty, form);

                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError("E-mail Failed To Send");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "E-mail Failed To Send");
            }
        }
    }
}
