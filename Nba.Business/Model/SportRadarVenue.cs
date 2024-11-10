namespace Nba.Business
{
    public class SportRadarVenue
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public int? Capacity { get; set; }

        public string? Address { get; set; }

        public required string City { get; set; }

        public string? State { get; set; }

        public string? Zip { get; set; }

        public required string Country { get; set; }

        public SportRadarLocation? Location { get; set; }

        public string? Sr_Id { get; set; }
    }
}
