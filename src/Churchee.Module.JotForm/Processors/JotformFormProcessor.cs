using Churchee.Common.Abstractions.Utilities;
using Churchee.Module.Tokens.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Churchee.Module.Jotform.Processors
{
    public partial class JotformFormProcessor : IFormProcessor
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DbContext _dbContext;
        private readonly ILogger _logger;

        public JotformFormProcessor(IHttpClientFactory httpClientFactory, DbContext dbContext, ILogger<JotformFormProcessor> logger)
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> ProcessForm(IDictionary<string, string> formData)
        {
            try
            {
                if (!formData.TryGetValue("formID", out string formId) ||
                    formId.Length > 20)
                {
                    return false;
                }

                if (!long.TryParse(formId, out long parsedFormId))
                {
                    return false;
                }

                var jotFormData = ConvertToJotformFormat(formData);

                string apiKey = await _dbContext.Set<Token>().AsQueryable().Where(w => w.Key == "Jotform").Select(s => s.Value).FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogError("Jotform API key missing");
                    return false;
                }

                var response = await PostToJotformApi(parsedFormId, jotFormData, apiKey);

                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();

                    if (_logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning(
                            "Jotform submission failed. Status: {Status}, Body: {Body}",
                            response.StatusCode, body);
                    }

                    return false;
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error submitting Jotform form");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Jotform request timed out");
                return false;
            }
        }

        private async Task<HttpResponseMessage> PostToJotformApi(long parsedFormId, Dictionary<string, object> jotFormData, string apiKey)
        {
            var httpClient = _httpClientFactory.CreateClient();

            httpClient.DefaultRequestHeaders.Add("APIKEY", apiKey);

            httpClient.BaseAddress = new Uri("https://api.jotform.com/");

            string json = JsonSerializer.Serialize(jotFormData);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"form/{parsedFormId}/submissions", content);
            return response;
        }

        public static Dictionary<string, object> ConvertToJotformFormat(IDictionary<string, string> fields)
        {
            var result = new Dictionary<string, object>();

            foreach (var kv in fields)
            {
                string key = kv.Key;
                string value = kv.Value;

                // Match patterns like q6_name[first]
                var match = JotFormFieldNumberPatern().Match(key);

                if (!match.Success)
                {
                    continue;
                }

                string id = match.Groups["id"].Value;

                string sub = match.Groups["sub"].Success ? match.Groups["sub"].Value : null;

                if (!result.TryGetValue(id, out object value1))
                {
                    value1 = sub == null ? value : new Dictionary<string, string>();
                    result[id] = value1;
                }

                if (sub != null)
                {
                    var obj = (Dictionary<string, string>)value1;
                    obj[sub] = value;
                }
            }

            return result;
        }

        [GeneratedRegex(@"^q(?<id>\d+)(?:_[^\[]+)?(?:\[(?<sub>[^\]]+)\])?$")]
        private static partial Regex JotFormFieldNumberPatern();
    }
}
