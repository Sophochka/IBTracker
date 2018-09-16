using SQLite;
using IBTracker.Common;

namespace IBTracker.Data
{
    public class School
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string Name { get; set; }
        public string Site { get; set; }
        public bool PYP { get; set; }
        public bool MYP { get; set; }
        public bool DP { get; set; }
        public bool CP { get; set; }
        public string Languages { get; set; }

        public School(IBSchool school)
        {
            Name = school.Name;
            Site = school.Site.AbsoluteUri;
            PYP = school.PYP;
            MYP = school.MYP;
            DP = school.DP;
            CP = school.CP;
            Languages = school.Languages;
        }
    }
}