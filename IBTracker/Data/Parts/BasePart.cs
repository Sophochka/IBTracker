using SQLite;

namespace IBTracker.Data
{
    public class BasePart
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}