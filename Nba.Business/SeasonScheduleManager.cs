using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nba.Business.Interface;
using Nba.Data.Models;

namespace Nba.Business
{
    public class SeasonScheduleManager : ISeasonScheduleManager
    {
        private readonly ILogger<SeasonScheduleManager> _logger;
        private readonly AppSettings _appSettings;
        private readonly ISportRadarService _service;
        private readonly NbaSqlContext _dbContext;

        public SeasonScheduleManager(
            ILogger<SeasonScheduleManager> logger, 
            IOptions<AppSettings> appSettings, 
            ISportRadarService service, 
            NbaSqlContext dbContext)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _service = service;
            _dbContext = dbContext;
        }

        public async Task<bool> GetSeasonSchedule()
        {
            try 
            {
                _logger.LogTrace("Getting season schedule...");
                IEnumerable<SportRadarGame> results = await _service.GetSeasonSchedule();
                await LoadScheduleToDatabase(results);

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error getting season schedule: {ex.Message}");
                return false;
            }
        }

        private async Task LoadScheduleToDatabase(IEnumerable<SportRadarGame> results)
        {
            _logger.LogTrace($"Iterating through results to add/update database as appropriate.");

            List<Game> existingGames = _dbContext.Games.ToList();
            List<Team> existingTeams = _dbContext.Teams.ToList();
            List<Venue> existingVenues = _dbContext.Venues.ToList();

            _logger.LogInformation($"Database currently contains {existingGames.Count} games, {existingTeams.Count} teams, and {existingVenues.Count} venues.");

            foreach (SportRadarGame result in results)
            {
                Team homeTeam = PrepareTeam(existingTeams, result.Home);
                Team awayTeam = PrepareTeam(existingTeams, result.Away);
                Venue venue = PrepareVenue(existingVenues, result);
                PrepareGame(existingGames, _appSettings.SeasonYear, result, homeTeam, awayTeam, venue);
            }

            _logger.LogTrace("Saving changes to database...");

            LogUpdateSummary();

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Database updated successfully!");
        }

        private void LogUpdateSummary()
        {
            int newTeams = _dbContext.ChangeTracker.Entries<Team>().Count(e => e.State == EntityState.Added);
            int updatedTeams = _dbContext.ChangeTracker.Entries<Team>().Count(e => e.State == EntityState.Modified);

            int newGames = _dbContext.ChangeTracker.Entries<Game>().Count(e => e.State == EntityState.Added);
            int updatedGames = _dbContext.ChangeTracker.Entries<Game>().Count(e => e.State == EntityState.Modified);

            int newVenues = _dbContext.ChangeTracker.Entries<Venue>().Count(e => e.State == EntityState.Added);
            int updatedVenues = _dbContext.ChangeTracker.Entries<Venue>().Count(e => e.State == EntityState.Modified);

            _logger.LogInformation($"Adding {newGames} games, {newTeams} teams, and {newVenues} venues. " +
                $"Updating {updatedGames} games, {updatedTeams} teams, and {updatedVenues} venues.");
        }

        private Game PrepareGame(IEnumerable<Game> existingGames, int year, SportRadarGame game, Team homeTeam, Team awayTeam, Venue venue)
        {
            // Check the local cache first
            Game? dbGame = _dbContext.Games.Local.FirstOrDefault(g => g.SportRadarGuid == game.Id);

            // If not found in the local cache, check the database data
            dbGame ??= existingGames.FirstOrDefault(g => g.SportRadarGuid == game.Id);

            // If still not found, create a new game
            if (dbGame == null)
            {
                dbGame = new();
                dbGame.SportRadarId = game.Sr_Id;
                dbGame.SportRadarGuid = game.Id;
                _dbContext.Games.Add(dbGame);
            }

            dbGame.Date = game.Scheduled;
            dbGame.SeasonId = year;
            dbGame.HomeTeam = homeTeam;
            dbGame.AwayTeam = awayTeam;
            dbGame.Status = game.Status;
            dbGame.HomeTeamScore = game.Home_Points;
            dbGame.AwayTeamScore = game.Away_Points;
            dbGame.Venue = venue;

            return dbGame;
        }

        private Team PrepareTeam(IEnumerable<Team> existingTeams, SportRadarTeam team)
        {
            // Check the local cache first
            Team? dbTeam = _dbContext.Teams.Local.FirstOrDefault(t => t.SportRadarGuid == team.Id);

            // If not found in the local cache, check the database data
            dbTeam ??= existingTeams.FirstOrDefault(t => t.SportRadarGuid == team.Id);
            
            // If still not found, create a new team
            if (dbTeam == null)
            {
                dbTeam = new();
                dbTeam.SportRadarId = team.Sr_Id;
                dbTeam.SportRadarGuid = team.Id;
                _dbContext.Teams.Add(dbTeam);
            }

            dbTeam.Name = team.Name;
            dbTeam.Alias = team.Alias;

            return dbTeam;
        }

        private Venue? PrepareVenue(IEnumerable<Venue> existingVenues, SportRadarGame game)
        {
            if (game.Venue == null)
            {
                return null;
            }

            SportRadarVenue venue = game.Venue;

            // Check the local cache first
            Venue? dbVenue = _dbContext.Venues.Local.FirstOrDefault(v => v.SportRadarGuid == venue.Id);

            // If not found in the local cache, check the database data
            dbVenue ??= existingVenues.FirstOrDefault(v => v.SportRadarGuid == venue.Id);

            // If still not found, create a new venue
            if (dbVenue == null)
            {
                dbVenue = new();
                dbVenue.SportRadarId = venue.Sr_Id;
                dbVenue.SportRadarGuid = venue.Id;
                _dbContext.Venues.Add(dbVenue);
            }

            dbVenue.Name = venue.Name;
            dbVenue.Address = venue.Address;
            dbVenue.City = venue.City;
            dbVenue.State = venue.State;
            dbVenue.Zip = venue.Zip;
            dbVenue.Country = venue.Country;
            dbVenue.Capacity = venue.Capacity;
            dbVenue.Latitude = venue.Location?.Lat;
            dbVenue.Longitude = venue.Location?.Lng;
            dbVenue.TimeZone = game.Time_Zones?.Venue;

            return dbVenue;
        }
    }
}
