using SQLite;

namespace IBTracker.Data
{
    [Table("Schools")]
    public class SchoolPart : BasePart
    {
        [Unique]
        public string Name { get; set; }

        public string Site { get; set; }

        // Primary Years Programme
        public bool PYP { get; set; }

        // Middle Years Programme
        public bool MYP { get; set; }

        // Diploma Programme
        public bool DP { get; set; }

        // Career-related Programme
        public bool CP { get; set; }

        public string Languages { get; set; }

        [Ignore]
        public bool AllProgramm => PYP && MYP && DP && CP;
    }
}