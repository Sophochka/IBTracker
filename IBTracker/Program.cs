using System;
using System.IO;
using System.Linq;
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
        private static readonly Logger.Level LogLevel = Logger.Level.Info;

        private static SearchFields SearchFields = new SearchFields
        {
            Region = "ibaem",
            Country = "PL",
            Keywords = "",
            Language = "",
            BoardingFacilities = "",
            SchoolGender = "",
        };

        static void Main(string[] args)
        {
            Logger.Init(LogLevel);
            Logger.Info($"Start search in country {SearchFields.Country}...");

            var parser = new SchoolParser();
            var schools = parser.Parse(SearchFields);
            Logger.Info($"Found {schools.Count()} schools. Add to database...");

            var database = new IBSchoolDb(DatabasePath);
            database.CreateTable(schools);

            //var scheets = new Sheets(AppName);
            //scheets.Test();

            Services.Clear();

            Logger.Info($"Press any key...");
            Console.ReadLine();
        }
    }
}
