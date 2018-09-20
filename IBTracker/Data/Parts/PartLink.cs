using SQLite;
using IBTracker.Contracts;

namespace IBTracker.Data
{
    [Table("Parts")]
    public class PartLink : BasePart
    {
        public int School { get; set; }
        public int Details { get; set; }
        public int Rating { get; set; }
    }
}