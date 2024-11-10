namespace Nba.Business
{
    public class SportRadarGame
    {
        public Guid Id { get; set; }

        public DateTime Scheduled { get; set; }

        public required string Status { get; set; }

        public int? Home_Points { get; set; }

        public int? Away_Points { get; set; }

        public string? Sr_Id { get; set; }

        public SportRadarTimeZones? Time_Zones { get; set; } 

        public SportRadarVenue? Venue { get; set; }

        public required SportRadarTeam Home { get; set; }

        public required SportRadarTeam Away { get; set; }
    }
}
