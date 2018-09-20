using IBTracker.Utils;

namespace IBTracker
{
    public class Config
    {
        public string AppName => "IBSchool_Tracker";
        public string DatabasePath => "./Data/Database/IBSchools.db";
        public string SearchIndex => SearchFields.ToString();
        public Logger.Level LogLevel => Logger.Level.Debug;

        public readonly SearchFields SearchFields = new SearchFields
        {
            Region = SchoolRegions.EuropeMEastAfrica,
            Country = SchoolCountries.Poland,
            Keywords = "",
            Language = "",
            BoardingFacilities = "",
            SchoolGender = "",
        };
    }
}