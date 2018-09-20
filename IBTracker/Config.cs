using IBTracker.Contracts;
using IBTracker.Data;
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

        public readonly HandlerParams[] Handlers;
        
        public Config()
        {
            Handlers = new[]
            {
                new HandlerParams(nameof(PartLink.School), new SchoolPartHandler(SearchFields), true, false, true),
                new HandlerParams(nameof(PartLink.Details), new DetailsPartHandler(), true, true),
                // new HandlerParams(nameof(PartLink.Rating), new RatingPartHandlerPL(), false, false),
            };
        }
    }

    public class HandlerParams
    {
        public string Name { get; }
        public IPartHandler Handler { get; }
        public bool ForceRetrieve { get; set; }
        public bool ForceLink { get; set; }
        public bool ClearIfRetrieve { get; }

        public HandlerParams(string name, IPartHandler handler, bool forceRetrieve, bool forceLink, bool clearIfRetrieve = false)
        {
            Name = name;
            Handler = handler;
            ForceRetrieve = forceRetrieve;
            ForceLink = forceLink;
            ClearIfRetrieve = clearIfRetrieve;
        }
    }
}