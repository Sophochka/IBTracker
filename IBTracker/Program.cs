using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using IBTracker.Contracts;
using IBTracker.Google;
using IBTracker.Data;
using IBTracker.Utils;

namespace IBTracker
{
    class Program
    {
        private const string AppName = "IBSchool_Tracker";
        private const string DatabasePath = "./Data/Database/IBSchools.db";
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

            var schoolInfo = new List<SchoolInfo>();
            var storage = new Storage(DatabasePath, SearchFields.ToString());
            HandleParts<SchoolPart>(schoolInfo, new SchoolPartHandler(SearchFields), storage, true, true);
            HandleParts<RatingPartPL>(schoolInfo, new RatingPartHandlerPL(), storage, false);

            //var scheets = new Sheets(AppName);
            //scheets.Test();

            Services.Clear();
            Logger.Info($"Press any key...");
            Console.ReadLine();
        }

        private static void HandleParts<T>(ICollection<SchoolInfo> schools, IPartHandler handler, Storage storage, bool forceRetrieve, bool clearIfRetrieve = false) where T : BasePart, new()
        {
            IEnumerable<T> parts = null;

            if (!forceRetrieve)
            {
                parts = storage.Read<T>();
                forceRetrieve = parts == null;
            }

            if (forceRetrieve)
            {
                parts = new List<T>(handler.Read(schools) as IEnumerable<T>);
                if (clearIfRetrieve) 
                {
                    storage.Clear();
                }  

                storage.Write(parts);
            }

            var name = typeof(T).Name;
            name = name.Substring(0, name.IndexOf("Part"));
            var action = forceRetrieve ? "Retrieved" : "Loaded";
            Logger.Info($"{action} {parts.Count()} {name} items.");

            handler.Link(schools, parts);
        } 
    }
}
