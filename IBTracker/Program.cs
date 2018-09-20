using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
            Logger.Info($"Collect for country {config.SearchFields.Country}");

            var storage = new Storage(config.DatabasePath, config.SearchIndex);
            var links = storage.Read<PartLink>();
            var schools = new List<SchoolInfo>();

            var forceLink = false;
            foreach (var parameters in config.Handlers)
            {
                HandlePart(schools, links, storage, parameters);
                forceLink |= parameters.ForceLink;
            }

            UpdateLinks(schools, links, storage, forceLink);

            //var scheets = new Sheets(AppName);
            //scheets.Test();

            Services.Clear();
            Logger.Info($"Press any key...");
            Console.ReadLine();
        }

        private static void HandlePart(ICollection<SchoolInfo> schools, IEnumerable<PartLink> links, Storage storage, HandlerParams item)
        {
            var type = typeof(Program);
            var method = type.GetMethod("HandleParts", BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(item.Handler.PartType);
            generic.Invoke(null, new object[] { schools, links, storage, item });
        }

        private static void HandleParts<T>(ICollection<SchoolInfo> schools, IEnumerable<PartLink> links, Storage storage, HandlerParams item) where T : BasePart, new()
        {
            IEnumerable<T> parts = null;
            Logger.Info($"Retrieve \"{item.Name}\" parts...", true);

            if (!item.ForceRetrieve)
            {
                parts = storage.Read<T>();
                item.ForceRetrieve = parts == null;
            }

            if (item.ForceRetrieve)
            {
                parts = new List<T>(item.Handler.Read(schools) as IEnumerable<T>);
                if (item.ClearIfRetrieve) 
                {
                    storage.Clear();
                }  

                storage.Write(parts);
            }

            var action = item.ForceRetrieve ? "Retrieved" : "Loaded";
            Logger.Info($"{action} {parts.Count()} items.");

            if (parts.Count() == 0)
            {
                Logger.Info($"Nothing to link.");
                return;
            }

            Logger.Info($"Link \"{item.Name}\" parts...", true);

            var count = 0;
            item.ForceLink |= item.ForceRetrieve || links == null;
            if (!item.ForceLink)
            {
                var partsDict = parts.ToDictionary(p => p.Id);
                foreach (var link in links)
                {

                }
            }

            if (item.ForceLink)
            {
                count = item.Handler.Link(schools, parts);
            }

            action = item.ForceLink ? "Linked" : "Restored";
            Logger.Info($"{action} {count} from {parts.Count()} items.");
        }

        private static void UpdateLinks(ICollection<SchoolInfo> schools, IEnumerable<PartLink> links, Storage storage, bool forceLink) 
        {
            if (links != null && !forceLink) return;

            if (links == null || forceLink)
            {
                links = schools.Select(s => new PartLink
                {
                    School = s.School.Id,
                    Details = s.Details?.Id ?? 0,
                    Rating = s.Rating?.Id ?? 0,
                });
            }

            storage.Write(links); 
        }
    }
}
