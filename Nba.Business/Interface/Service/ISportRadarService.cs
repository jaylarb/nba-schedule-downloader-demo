namespace Nba.Business.Interface
{
    public interface ISportRadarService
    {
        Task<IEnumerable<SportRadarGame>> GetSeasonSchedule();
    }
}
