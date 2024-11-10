namespace Nba.Business.Interface
{
    public interface ISeasonScheduleManager
    {
        Task<bool> GetSeasonSchedule();
    }
}
