using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using IBTracker.Contracts;
using IBTracker.Utils;

namespace IBTracker.Data
{
    public class SchoolPartHandler : IPartHandler
    {
        private const int CellInRow = 6;
        private const string PageParameterName = "page";
        private const string PartialParameterName = "partial";
        private const string PartialPageParameterName = "partialPage";

        private static Uri IBOUri = new Uri("https://ibo.org");
        private static Uri SchoolSearchUri = new Uri(IBOUri, "programmes/find-an-ib-school/");
        private const string SchoolTablePath = "//*[@id=\"mainContent\"]/div[3]/div/div/table/tbody";

        private const string QueryTrue = "true";
        private const string QueryFalse = "false";

        private SearchFields fields;

        public Type PartType => typeof(SchoolPart);

        public SchoolPartHandler(SearchFields fields)
        {
            this.fields = fields;
        }

        public IEnumerable<BasePart> Read(IDictionary<int, SchoolInfo> schools)
        {
            var parts = new List<SchoolPart>();
            Extensions.ParsePages(
                (page) => BuildUri(fields, page), 
                (document) => 
                {
                    var items = document.ParseTable<SchoolPart>(SchoolTablePath, ParseRow);
                    parts.AddRange(items);
                    return !items.Any();
                });

            return parts;
        }

        public int Link(IDictionary<int, SchoolInfo> schools, IDictionary<int, BasePart> parts)
        {
            schools.Clear();
            foreach (var part in parts)
            {
                schools.Add(
                    part.Key,
                    new SchoolInfo
                    {
                        School = part.Value as SchoolPart
                    });
            }

            return schools.Count;
        }

        private Uri BuildUri(SearchFields fields, int page)
        {
            var builder = new UriBuilder(SchoolSearchUri);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["SearchFields.Region"] = fields.Region;
            query["SearchFields.Country"] = fields.Country;
            query["SearchFields.Keywords"] = fields.Keywords;
            query["SearchFields.Language"] = fields.Language;
            query["SearchFields.BoardingFacilities"] = fields.BoardingFacilities;
            query["SearchFields.SchoolGender"] = fields.SchoolGender;
            if (page > 1)
            {
                query[PartialParameterName] = QueryTrue;
                query[PageParameterName] = page.ToString();
            }

            query[PartialPageParameterName] = QueryFalse;

            builder.Query = query.ToString();
            return builder.Uri;
        }

        private SchoolPart ParseRow(HtmlNodeCollection rowCells)
        {
            if (rowCells == null || rowCells.Count != CellInRow) return null;
            if (!rowCells[0].ParseLink(out string fullName, out string uri)) return null;

            SplitNameAndCity(fullName, out string name, out string city);
            var school = new SchoolPart()
            {
                FullName = fullName,
                Name = name,
                City = city,
                Site = new Uri(IBOUri, uri).AbsoluteUri,
                PYP = rowCells[1].ParseCheck(HTML.SPAN),
                MYP = rowCells[2].ParseCheck(HTML.SPAN),
                DP = rowCells[3].ParseCheck(HTML.SPAN),
                CP = rowCells[4].ParseCheck(HTML.SPAN),
            };

            var languages = rowCells[5].InnerText.Split("\r\n").Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l));
            school.Languages = string.Join(",", languages);
            return school;
        }

        private void SplitNameAndCity(string fullName, out string name, out string city)
        {
            city = "";
            name = fullName;
            string[] delimiters = new[] { " of ", " w ", "," };
            foreach (var delimiter in delimiters)
            {
                var pos = fullName.IndexOf(delimiter);
                if (pos != -1)
                {
                    name = fullName.Substring(0, pos).Trim();
                    city = fullName.Substring(pos + delimiter.Length).Trim();
                    break;
                }
            }
        }
    }
}
