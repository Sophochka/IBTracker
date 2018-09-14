using System;
using System.Linq;
using IBOTracker.Google;
using IBOTracker.Parsing;

namespace IBOTracker
{
    class Program
    {
        private const string AppName = "IBO_School_Tracker";
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
            Logger.Info($"Found {schools.Count()} schools");

            var scheets = new Sheets(AppName);
            scheets.Test();

            Services.Clear();
            Console.ReadLine();
        }
    }
}
