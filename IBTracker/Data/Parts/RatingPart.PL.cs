using SQLite;
using IBTracker.Contracts;

namespace IBTracker.Data
{
    [Table("Ratings")]
    public class RatingPartPL : BasePart
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Woj { get; set; }
        public int P15 { get; set; }
        public int P16 { get; set; }
        public int P17 { get; set; }
        public int P18 { get; set; }
        public decimal B18 { get; set; }
        public string Znak { get; set; }
    }
}