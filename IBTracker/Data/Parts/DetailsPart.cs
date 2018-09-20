using SQLite;
using IBTracker.Contracts;

namespace IBTracker.Data
{
    [Table("Details")]
    public class DetailsPart : BasePart
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
}