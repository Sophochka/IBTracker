using SQLite;
using IBTracker.Contracts;

namespace IBTracker.Data
{
    [Table("Details")]
    public class DetailsPart : BasePart
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public string Head { get; set; }
        public string Site { get; set; }
        public string DateSince { get; set; }
        public string DateAuth { get; set; }
        public string Language { get; set; }
        public string Gender { get; set; }
        public string Facilities { get; set; }
    }
}