using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using HtmlAgilityPack;
using IBTracker.Contracts;
using IBTracker.Utils;

namespace IBTracker.Data
{
    public class DetailsPartHandler : IPartHandler
    {
        public Type PartType => typeof(DetailsPart);

        public IEnumerable<BasePart> Read(IDictionary<int, SchoolInfo> schools)
        {
            var count = 1;
            var webClient = new WebClient();
            var parts = new List<DetailsPart>();
            Logger.BeginProgress(schools.Count);
            foreach(var info in schools.Values)
            {
                var document = new HtmlDocument();
                document.Load(webClient.OpenRead(info.School.Site), Encoding.UTF8);
                parts.Add(ParsePage(document));
                Logger.SetProgress(count++);
            }

            Logger.EndProgress();
            return parts;
        }

        public int Link(IDictionary<int, SchoolInfo> schools, IDictionary<int, BasePart> parts)
        {
            return 0;
        }

        private DetailsPart ParsePage(HtmlDocument document)
        {
            var values = GetValues(document).ToList();

            // https://www.google.com/maps/?q=51.2499,22.5357
            var detailsPart = new DetailsPart
            {
                Code = GetValue(values, "code:"),
                Type = GetValue(values, "Type:"),
                Head = GetValue(values, "Head"),
                Site = GetValue(values, "Website:"),
                DateSince = GetValue(values, "since:"),
                DateAuth = GetValue(values, "Authorised:"),
                Language = GetValue(values, "Language"),
                Gender = GetValue(values, "Gender:"),
                Facilities = GetValue(values, "facilities:"),
            };

            return detailsPart;
        }

        private IEnumerable<Tuple<string, string>> GetValues(HtmlDocument document)
        {
            var values = new List<Tuple<string, string>>();
            values.AddRange(ParseValues(document.DocumentNode.SelectNodes("/html/body/main/div[2]/div/div[1]/div/div[1]")));
            values.AddRange(ParseValues(document.DocumentNode.SelectNodes("/html/body/main/div[2]/div/div[1]/div/div[2]")));
            return values.ToArray();
        }

        private IEnumerable<Tuple<string, string>> ParseValues(HtmlNodeCollection table)
        {
            var node = table.FirstOrDefault();
            var list = new List<Tuple<string, string>>();
            if (node == null) return Enumerable.Empty<Tuple<string, string>>();

            var items = node.InnerText.Split("\r\n").Select(v => v.Trim()).Where(v => !string.IsNullOrEmpty(v));
            var names = items.Where((c,i) => i % 2 == 0).ToArray();
            var values = items.Where((c,i) => i % 2 != 0).ToArray();
            if (names.Length != values.Length) return Enumerable.Empty<Tuple<string, string>>();

            return names.Select((c,i) => new Tuple<string, string>(c, values[i]));
        }

        private string GetValue(IEnumerable<Tuple<string, string>> values, string name)
        {
            var item = values.FirstOrDefault(i => i.Item1.Contains(name));
            return item?.Item2 ?? "";
        }
    }
}