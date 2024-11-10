namespace Nba.Business
{
    public class AppSettings
    {
        public required string SeasonType { get; set; }

        public required int SeasonYear { get; set; }

        public required string SportRadarApiKey { get; set; }

        public required string SportRadarApiUrl { get; set; }

    }
}
