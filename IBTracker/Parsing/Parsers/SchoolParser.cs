using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;

namespace IBOTracker.Parsing
{
    public class SchoolParser
    {
        public IEnumerable<IBSchool> Parse(SearchFields fields)
        {
            var schools = new List<IBSchool>();
            ParserExtensions.ParsePages(
                (page) => BuildUri(fields, page), 
                (document) => 
                {
                    var items = document.ParseTable(Consts.SchoolTablePath, RowToSchool);
                    schools.AddRange(items);
                    return !items.Any();
                });

            return schools;
        }

        private Uri BuildUri(SearchFields fields, int page)
        {
            var builder = new UriBuilder(Consts.SchoolSearchUri);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query[SearchFieldNames.Region] = fields.Region;
            query[SearchFieldNames.Country] = fields.Country;
            query[SearchFieldNames.Keywords] = fields.Keywords;
            query[SearchFieldNames.Language] = fields.Language;
            query[SearchFieldNames.BoardingFacilities] = fields.BoardingFacilities;
            query[SearchFieldNames.SchoolGender] = fields.SchoolGender;
            if (page > 1)
            {
                query[Consts.PartialParameterName] = Consts.True;
                query[Consts.PageParameterName] = page.ToString();
            }

            query[Consts.PartialPageParameterName] = Consts.False;

            builder.Query = query.ToString();
            return builder.Uri;
        }

        private IBSchool RowToSchool(HtmlNodeCollection rowCells)
        {
            if (rowCells == null || rowCells.Count != Consts.SchoolTableCellInRow) return null;
            if (!rowCells[0].ParseLink(out string name, out string uri)) return null;

            var school = new IBSchool
            {
                Name = name,
                Site = new Uri(Consts.IBOUri, uri),
                PYP = rowCells[1].ParseCheck(HTML.SPAN),
                MYP = rowCells[2].ParseCheck(HTML.SPAN),
                DP = rowCells[3].ParseCheck(HTML.SPAN),
                CP = rowCells[4].ParseCheck(HTML.SPAN),
            };

            school.SetLanguages(rowCells[5].InnerText);
            return school;
        }
    }
}
