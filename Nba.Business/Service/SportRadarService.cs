using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nba.Business.Interface;
using Newtonsoft.Json;

namespace Nba.Business
{
    public class SportRadarService : ISportRadarService
    {   
        private readonly AppSettings _appSettings;
        private readonly IHttpClientWrapper _httpClient;
        private readonly ILogger<SportRadarService> _logger;

        public SportRadarService(ILogger<SportRadarService> logger, IOptions<AppSettings> appSettings, IHttpClientWrapper httpClient)
        {
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<SportRadarGame>> GetSeasonSchedule()
        {
            try
            {
                IEnumerable<SportRadarGame> games = new List<SportRadarGame>();
                string url = $"{_appSettings.SportRadarApiUrl}games/{_appSettings.SeasonYear}/{_appSettings.SeasonType}/schedule.json";

                _logger.LogTrace($"Getting season schedule from {url}...");

                HttpResponseMessage response = await GetResponse(url);

                string json = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var converted = JsonConvert.DeserializeObject<SportRadarScheduleResult>(json);
                    games = converted?.Games ?? games;
                }

                _logger.LogInformation($"Season schedule from {url} retrieved successfully. Found {games.Count()} records.");

                return games;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting season schedule from sport radar: {ex.Message}");
                throw;
            }
        }

        private async Task<HttpResponseMessage> GetResponse(string url)
        {
            string urlWithKey = $"{url}?api_key={_appSettings.SportRadarApiKey}";
            HttpResponseMessage response = await _httpClient.GetAsync(urlWithKey);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = $"Failed to get response from ${url}.";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            return response;
        }
    }
}
