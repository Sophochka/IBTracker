using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using IBTracker.Common;
using IBTracker.Google;
using IBTracker.Parsing;
using IBTracker.Data;

namespace IBTracker
{
    class Program
    {
        private const string AppName = "IBO_School_Tracker";
        private const string DatabasePath = "./Database/IBSchools.db";
        private static readonly Logger.Level LogLevel = Logger.Level.Debug;

        private static SearchFields SearchFields = new SearchFields
        {
            Region = "ibaem",
            Country = "PL",
            Keywords = "",
            Language = "",
            BoardingFacilities = "",
            SchoolGender = "",
        };

        private static void Main(string[] args)
        {
            Logger.Init(LogLevel);
            Logger.Info($"Collect for country {SearchFields.Country}...");

            var database = new IBSchoolDb(DatabasePath, SearchFields.ToString());
            var schools = RetrieveSchools(SearchFields, database, false);
            Logger.Info($"Found {schools.Count()} schools.");

            //var scheets = new Sheets(AppName);
            //scheets.Test();

            Services.Clear();
            Logger.Info($"Press any key...");
            Console.ReadLine();
        }

        private static IEnumerable<IBSchool> RetrieveSchools(SearchFields fields, IBSchoolDb db, bool forceRetrieve)
        {
            if (!forceRetrieve)
            {
                var stored = db.GetSchools();
                if (stored.Any())
                {
                    return stored;
                }
            }

            var parser = new SchoolParser();
            var schools = parser.Parse(fields);
            db.CreateTable(schools);
            return schools;
        } 
    }
}
