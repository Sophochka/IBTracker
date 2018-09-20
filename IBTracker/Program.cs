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
        private static void Main(string[] args)
        {
            var config = new Config();

            Logger.Init(config.LogLevel);
            Logger.Info($"Collect for country {config.SearchFields.Country}...");

            var schoolInfo = new List<SchoolInfo>();
            var storage = new Storage(config.DatabasePath, config.SearchIndex);
            var links = storage.Read<PartLink>();

            HandleParts<SchoolPart>(schoolInfo, links, new SchoolPartHandler(config.SearchFields), storage, true, true);
            HandleParts<RatingPartPL>(schoolInfo, links, new RatingPartHandlerPL(), storage, false);

            //var scheets = new Sheets(AppName);
            //scheets.Test();

            Services.Clear();
            Logger.Info($"Press any key...");
            Console.ReadLine();
        }

        private static void HandleParts<T>(ICollection<SchoolInfo> schools, IEnumerable<PartLink> links, IPartHandler handler, Storage storage, bool forceRetrieve, bool clearIfRetrieve = false) where T : BasePart, new()
        {
            IEnumerable<T> parts = null;
            var partName = typeof(T).Name;
            partName = partName.Substring(0, partName.IndexOf("Part"));
            Logger.Info($"Start part \"{partName}\"...");

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

            var action = forceRetrieve ? "Retrieved" : "Loaded";
            Logger.Info($"{action}: {parts.Count()}");

            var count = handler.Link(schools, links, parts);
            Logger.Info($"Linked {count} from {parts.Count()}");
        } 
    }
}
