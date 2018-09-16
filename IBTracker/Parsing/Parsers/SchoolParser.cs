using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using IBTracker.Common;

namespace IBTracker.Parsing
{
    public class SchoolParser
    {
        private const int SchoolTableCellInRow = 6;
        private const string PageParameterName = "page";
        private const string PartialParameterName = "partial";
        private const string PartialPageParameterName = "partialPage";

        private static Uri IBOUri = new Uri("https://ibo.org");
        private static Uri SchoolSearchUri = new Uri(IBOUri, "programmes/find-an-ib-school/");
        private const string SchoolTablePath = "//*[@id=\"mainContent\"]/div[3]/div/div/table/tbody";

        private const string QueryTrue = "true";
        private const string QueryFalse = "false";

        public IEnumerable<IBSchool> Parse(SearchFields fields)
        {
            var schools = new List<IBSchool>();
            Extensions.ParsePages(
                (page) => BuildUri(fields, page), 
                (document) => 
                {
                    var items = document.ParseTable(SchoolTablePath, ParseRow);
                    schools.AddRange(items);
                    return !items.Any();
                });

            return schools;
        }

        private Uri BuildUri(SearchFields fields, int page)
        {
            var builder = new UriBuilder(SchoolSearchUri);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query[SearchFieldNames.Region] = fields.Region;
            query[SearchFieldNames.Country] = fields.Country;
            query[SearchFieldNames.Keywords] = fields.Keywords;
            query[SearchFieldNames.Language] = fields.Language;
            query[SearchFieldNames.BoardingFacilities] = fields.BoardingFacilities;
            query[SearchFieldNames.SchoolGender] = fields.SchoolGender;
            if (page > 1)
            {
                query[PartialParameterName] = QueryTrue;
                query[PageParameterName] = page.ToString();
            }

            query[PartialPageParameterName] = QueryFalse;

            builder.Query = query.ToString();
            return builder.Uri;
        }

        private IBSchool ParseRow(HtmlNodeCollection rowCells)
        {
            if (rowCells == null || rowCells.Count != SchoolTableCellInRow) return null;
            if (!rowCells[0].ParseLink(out string name, out string uri)) return null;

            var school = new IBSchool
            {
                Name = name,
                Site = new Uri(IBOUri, uri),
                PYP = rowCells[1].ParseCheck(HTML.SPAN),
                MYP = rowCells[2].ParseCheck(HTML.SPAN),
                DP = rowCells[3].ParseCheck(HTML.SPAN),
                CP = rowCells[4].ParseCheck(HTML.SPAN),
            };

            var languages = rowCells[5].InnerText.Split("\r\n").Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l));
            school.Languages = string.Join(",", languages);
            return school;
        }
    }
}
