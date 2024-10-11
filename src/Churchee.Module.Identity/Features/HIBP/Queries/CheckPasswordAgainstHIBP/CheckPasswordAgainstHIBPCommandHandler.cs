using Churchee.Common.ResponseTypes;
using MediatR;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Features.HIBP.Queries
{
    public class CheckPasswordAgainstHIBPCommandHandler : IRequestHandler<CheckPasswordAgainstHIBPCommand, CommandResponse>
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public CheckPasswordAgainstHIBPCommandHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CommandResponse> Handle(CheckPasswordAgainstHIBPCommand request, CancellationToken cancellationToken)
        {

            var commandResponse = new CommandResponse();

            if (string.IsNullOrEmpty(request.Password))
            {
                commandResponse.AddError("No Password Provided", "Password");

                return commandResponse;
            }

            try
            {

                string hash = ComputeSha1Hash(request.Password);
                string hashPrefix = hash[..5];
                var hashSuffix = hash.Substring(5).ToUpper();

                var response = await _httpClientFactory.CreateClient().GetAsync($"https://api.pwnedpasswords.com/range/{hashPrefix}");

                if (!response.IsSuccessStatusCode)
                {
                    commandResponse.AddError("Error Calling hibp API", "Password");

                    return commandResponse;
                }

                var content = await response.Content.ReadAsStringAsync();

                var lines = content.Split("\r\n");

                foreach (var line in lines.Where(w => !string.IsNullOrEmpty(w)))
                {
                    var parts = line.Split(':');

                    var suffix = parts[0];
                    var count = int.Parse(parts[1]);

                    if (hashSuffix == suffix)
                    {
                        commandResponse.AddError($"Password found in leaked passwords database (HIBP) {count} times!. Please try a different password, ideally generated via a password manager", "Password");

                        return commandResponse;
                    }
                }

            }
            catch (Exception ex)
            {
                commandResponse.AddError("Error Calling HIBP API", "");
            }

            return commandResponse;
        }

        private string ComputeSha1Hash(string input)
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            var hashBytes = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
