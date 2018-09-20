using SQLite;

namespace IBTracker.Contracts
{
    public class BasePart
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}