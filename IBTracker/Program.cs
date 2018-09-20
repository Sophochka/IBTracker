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
            var schools = new Dictionary<int, SchoolInfo>();

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

        private static void HandlePart(IDictionary<int, SchoolInfo> schools, IEnumerable<PartLink> links, Storage storage, HandlerParams item)
        {
            var type = typeof(Program);
            var method = type.GetMethod("HandleParts", BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(item.Handler.PartType);
            generic.Invoke(null, new object[] { schools, links, storage, item });
        }

        private static void HandleParts<T>(IDictionary<int, SchoolInfo> schools, IEnumerable<PartLink> links, Storage storage, HandlerParams prms) where T : BasePart, new()
        {
            Logger.Info($"Retrieve \"{prms.Name}\" parts...", true);
            var parts = RetrieveParts<T>(schools, links, storage, prms);

            var action = prms.ForceRetrieve ? "Retrieved" : "Loaded";
            Logger.Info($"{action} {parts.Count()} items.");

            if (parts.Count() == 0)
            {
                Logger.Info($"Nothing to link.");
                return;
            }

            Logger.Info($"Link \"{prms.Name}\" parts...", true);
            var count = LinkParts(schools, links, parts.ToDictionary<BasePart>(), prms);

            action = (count == -1 ? "ERROR: " : "") + (prms.ForceLink ? "Linked" : "Restored");
            Logger.Info($"{action} {count} from {parts.Count()} items.");
        }

        private static IEnumerable<T> RetrieveParts<T>(IDictionary<int, SchoolInfo> schools, IEnumerable<PartLink> links, Storage storage, HandlerParams prms) where T : BasePart, new()
        {
            IEnumerable<T> parts = null;

            if (!prms.ForceRetrieve)
            {
                parts = storage.Read<T>();
                prms.ForceRetrieve = parts == null;
            }

            if (prms.ForceRetrieve)
            {
                parts = new List<T>(prms.Handler.Read(schools) as IEnumerable<T>);
                if (prms.ClearIfRetrieve) 
                {
                    storage.Clear();
                }  

                storage.Write(parts);
            }

            return parts;
        }

        private static int LinkParts(IDictionary<int, SchoolInfo> schools, IEnumerable<PartLink> links, IDictionary<int, BasePart> parts, HandlerParams item)
        {
            if (parts.Count() == 0) return -1;

            var count = 0;
            item.ForceLink |= item.ForceRetrieve || links == null || !links.Any();
            if (!item.ForceLink)
            {
                var propInfo = typeof(SchoolInfo).GetProperty(item.Name, BindingFlags.Public | BindingFlags.Instance);
                var propLink = typeof(PartLink).GetProperty(item.Name, BindingFlags.Public | BindingFlags.Instance);
                if (!propInfo?.CanWrite ?? false || propLink == null) return -1;

                foreach (var link in links)
                {
                    SchoolInfo info;
                    if (!schools.TryGetValue(link.School, out info)) continue;

                    var id = propLink.GetValue(link) as int?;
                    if (id == null) continue;

                    BasePart part;
                    if (!parts.TryGetValue(id.Value, out part)) continue;

                    propInfo.SetValue(info, part, null);
                    count++;
                }
            }

            if (item.ForceLink)
            {
                count = item.Handler.Link(schools, parts);
            }

            return count;
        }

        private static void UpdateLinks(IDictionary<int, SchoolInfo> schools, IEnumerable<PartLink> links, Storage storage, bool forceLink) 
        {
            if (links != null && !forceLink) return;

            if (links == null || forceLink)
            {
                links = schools.Values.Select(s => new PartLink
                {
                    School = s.School.Id,
                    Details = s.Details?.Id ?? 0,
                    Rating = s.Rating?.Id ?? 0,
                });
            }

            Logger.Info($"Update links...", true);
            storage.Write(links); 
        }
    }
}
