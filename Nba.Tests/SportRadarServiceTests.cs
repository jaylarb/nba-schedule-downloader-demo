using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nba.Business.Interface;
using System.Net;

namespace Nba.Business.Tests
{
    public class SportRadarServiceTests
    {
        private readonly Mock<ILogger<SportRadarService>> _mockLogger = new();
        private readonly Mock<IOptions<AppSettings>> _mockSettings = new();
        private readonly Mock<IHttpClientWrapper> _mockHttpClient = new();

        private SportRadarService _service;

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

            _service = new SportRadarService(_mockLogger.Object, _mockSettings.Object, _mockHttpClient.Object);
        }

        [Test]
        public async Task GetSeasonSchedule_ExpectedData_SuccessfullyConvertsFromJson()
        {
            // Arrange
            string sampleData = File.ReadAllText("sample-data/sport-radar-preseason-schedule.json");
            HttpResponseMessage okResponse = new()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(sampleData)
            };

            _mockHttpClient.Setup(service => service.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(okResponse);

            // Act
            var result = await _service.GetSeasonSchedule();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(74));
        }
    }
}