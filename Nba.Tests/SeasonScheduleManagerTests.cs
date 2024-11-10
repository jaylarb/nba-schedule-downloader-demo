using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nba.Business.Interface;
using Nba.Data.Models;

namespace Nba.Business.Tests
{
    public class SeasonScheduleManagerTests
    {
        private readonly Mock<ILogger<SeasonScheduleManager>> _mockLogger = new();
        private readonly Mock<IOptions<AppSettings>> _mockSettings = new();
        private NbaSqlContext _dbContext;
        private readonly Mock<ISportRadarService> _mockService = new();
        private SeasonScheduleManager _manager;

        [SetUp]
        public void Setup()
        {
            _mockSettings.Setup(service => service.Value)
                .Returns(new AppSettings()
                {
                    SeasonYear = 2024,
                    SeasonType = "PRE",
                    SportRadarApiUrl = "abc",
                    SportRadarApiKey = "def"
                });

            var options = new DbContextOptionsBuilder<NbaSqlContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new NbaSqlContext(options);

            // Ensure the database is deleted to start with a clean slate
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _manager = new SeasonScheduleManager(_mockLogger.Object, _mockSettings.Object, _mockService.Object, _dbContext);
        }

        [Test]
        public async Task GetSeasonSchedule_ExpectedData_SuccessfullyLoadsToDatabase()
        {
            // Arrange
            SportRadarTeam team1 = new() {  Alias = "T1", Name = "Team 1", Id = Guid.NewGuid() };
            SportRadarTeam team2 = new() {  Alias = "T2", Name = "Team 2", Id = Guid.NewGuid() };
            SportRadarTeam team3 = new() {  Alias = "T3", Name = "Team 3", Id = Guid.NewGuid() };
            SportRadarVenue venue1 = new() { Name = "Venue 1", City = "City", Country = "Country", Id = Guid.NewGuid() };
            SportRadarVenue venue2 = new() { Name = "Venue 2", City = "City", Country = "Country", Id = Guid.NewGuid() };
            SportRadarGame game1 = new() { Venue = venue1, Scheduled = DateTime.UtcNow, Home = team1, Away = team2, Status = "Future", Id = Guid.NewGuid() };
            SportRadarGame game2 = new() { Venue = venue1, Scheduled = DateTime.UtcNow, Home = team1, Away = team3, Status = "Future", Id = Guid.NewGuid() };
            SportRadarGame game3 = new() { Venue = venue2, Scheduled = DateTime.UtcNow, Home = team2, Away = team1, Status = "Future", Id = Guid.NewGuid() };
            SportRadarGame game4 = new() { Venue = venue2, Scheduled = DateTime.UtcNow, Home = team3, Away = team1, Status = "Future", Id = Guid.NewGuid() };
            SportRadarGame game5 = new() { Venue = venue2, Scheduled = DateTime.UtcNow, Home = team3, Away = team2, Status = "Future", Id = Guid.NewGuid() };

            IEnumerable<SportRadarGame> games = new List<SportRadarGame> { game1, game2, game3, game4, game5 };

            _mockService.Setup(service => service.GetSeasonSchedule())
                .ReturnsAsync(games);

            // Act
            await _manager.GetSeasonSchedule();

            // Assert

            int countGames = _dbContext.Games.Count();
            int countTeams = _dbContext.Teams.Count();
            int countVenues = _dbContext.Venues.Count();

            var dbGames = _dbContext.Games.ToList();
            var dbTeams = _dbContext.Teams.ToList();
            var dbVenues = _dbContext.Venues.ToList();

            Assert.That(countGames, Is.EqualTo(5));
            Assert.That(countTeams, Is.EqualTo(3));
            Assert.That(countVenues, Is.EqualTo(2));
        }
    }
}