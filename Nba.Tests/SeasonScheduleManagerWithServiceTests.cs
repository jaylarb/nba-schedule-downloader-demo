using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nba.Business.Interface;
using Nba.Data.Models;
using System.Net;

namespace Nba.Business.Tests
{
    public class SeasonScheduleManagerWithServiceTests
    {
        private readonly Mock<ILogger<SeasonScheduleManager>> _mockLoggerManager = new();
        private readonly Mock<ILogger<SportRadarService>> _mockLoggerService = new();
        private readonly Mock<IOptions<AppSettings>> _mockSettings = new();
        private NbaSqlContext _dbContext;
        private readonly Mock<IHttpClientWrapper> _mockHttpClient = new();
        private SportRadarService _service;
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

            _service = new SportRadarService(_mockLoggerService.Object, _mockSettings.Object, _mockHttpClient.Object);

            _manager = new SeasonScheduleManager(_mockLoggerManager.Object, _mockSettings.Object, _service, _dbContext);
        }

        [Test]
        public async Task GetSeasonSchedule_EndToEndUsingSampleData_SuccessfullyLoadsToDatabase()
        {
            // Arrange
            string sampleData = File.ReadAllText("sample-data/sport-radar-preseason-schedule.json");
            //string sampleData = File.ReadAllText("sample-data/sport-radar-regular-schedule.json");
            HttpResponseMessage okResponse = new()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(sampleData)
            };

            _mockHttpClient.Setup(service => service.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(okResponse);

            // how can I reset the in memory database so that it starts with 0 records?


            // Act
            await _manager.GetSeasonSchedule();

            // Assert
            int countGames = _dbContext.Games.Count();
            int countTeams = _dbContext.Teams.Count();
            int countVenues = _dbContext.Venues.Count();

            Assert.That(countGames, Is.EqualTo(74));
            Assert.That(countTeams, Is.EqualTo(32));
            Assert.That(countVenues, Is.EqualTo(39));
        }
    }
}
