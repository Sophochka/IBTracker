using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using IBTracker.Common;
using IBTracker.Google;
using IBTracker.Parsing;
using IBTracker.Data;
using IBTracker.Data.Tables;

namespace IBTracker
{
    class Program
    {
        private const string AppName = "IBO_School_Tracker";
        private const string DatabasePath = "IBSchools.db"; //"./Database/IBSchools.db";
        private static readonly Logger.Level LogLevel = Logger.Level.Debug;

        private static SearchFields SearchFields = new SearchFields
        {
            Region = SchoolRegions.EuropeMEastAfrica,
            Country = SchoolCountries.Poland,
            Keywords = "",
            Language = "",
            BoardingFacilities = "",
            SchoolGender = "",
        };

        private static void Main(string[] args)
        {
            Logger.Init(LogLevel);
            Logger.Info($"Collect for country {SearchFields.Country}...");

            var storage = new SchoolStorage(DatabasePath, SearchFields.ToString());
            var schools = RetrieveSchools(SearchFields, storage, false);
            Logger.Info($"Found {schools.Count()} schools.");

            var ratings = RetrieveRating(storage, true);
            Logger.Info($"Found {ratings.Count()} ratings.");

            //var scheets = new Sheets(AppName);
            //scheets.Test();

            Services.Clear();
            Logger.Info($"Press any key...");
            Console.ReadLine();
        }

        private static IEnumerable<School> RetrieveSchools(SearchFields fields, SchoolStorage storage, bool forceRetrieve)
        {
            if (!forceRetrieve)
            {
                var stored = storage.Read<School>();
                if (stored.Any())
                {
                    return stored;
                }
            }

            var parser = new SchoolParser();
            var schools = parser.Parse(fields);
            storage.Write(schools, true);
            return schools;
        } 

        private static IEnumerable<RatingPL> RetrieveRating(SchoolStorage storage, bool forceRetrieve)
        {
            if (!forceRetrieve)
            {
                var stored = storage.Read<RatingPL>();
                if (stored.Any())
                {
                    return stored;
                }
            }

            var parser = new NationalRatingPL();
            var ratings = parser.Parse();
            storage.Write(ratings);
            return ratings;
        } 
    }
}
