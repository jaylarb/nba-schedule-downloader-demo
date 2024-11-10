namespace Nba.Business
{
    public class SportRadarTeam
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Alias { get; set; }

        public string? Sr_Id { get; set; }
    }
}
