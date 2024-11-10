namespace Nba.Data.Models
{
    public abstract class HasModifiedDates
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
