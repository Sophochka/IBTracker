using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using IBTracker.Contracts;
using IBTracker.Utils;

namespace IBTracker.Data
{
    public class RatingPartHandlerPL : IPartHandler
    {
        private const int CellInRow = 13;
        private const string PageParameterName = "strona";
        private static Uri Root = new Uri("http://www.perspektywy.pl");
        private static Uri PageUri = new Uri(Root, "portal/index.php?option=com_content&view=article&id=3718:ranking-liceow-ogolnoksztalcacych-2018&catid=240&Itemid=445");

        private const string RatingTablePath = "//*[@id=\"content_tabele\"]/table/tbody";

        public IEnumerable<BasePart> Read(ICollection<SchoolInfo> schools)
        {
            var ratings = new List<RatingPartPL>();
            Extensions.ParsePages(
                (page) => BuildPageUri(page), 
                (document) => 
                {
                    var items = document.ParseTable(RatingTablePath, ParseRow);
                    ratings.AddRange(items);
                    return !items.Any();
                });

            return ratings;
        }

        public void Link(ICollection<SchoolInfo> schools, IEnumerable<BasePart> parts)
        {
            

        }

        private Uri BuildPageUri(int page)
        {
            var builder = new UriBuilder(PageUri);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query[PageParameterName] = page.ToString();
            builder.Query = query.ToString();
            return builder.Uri;
        }

        private RatingPartPL ParseRow(HtmlNodeCollection rowCells)
        {
            if (rowCells == null || rowCells.Count != CellInRow) return null;
            var znak = rowCells[11].InnerHtml;

            var rating = new RatingPartPL
            {
                Name = rowCells[1].InnerText,
                City = rowCells[2].InnerText,
                Woj = rowCells[3].InnerText, 
                P15 = GetPosition(rowCells[6].InnerText),
                P16 = GetPosition(rowCells[5].InnerText),
                P17 = GetPosition(rowCells[4].InnerText),
                P18 = GetPosition(rowCells[0].InnerText),
                B18 = Convert.ToDecimal(rowCells[7].InnerText.Trim().Replace('.', ',')),
                Znak = znak.Contains("gold") ? "GOLD" : znak.Contains("silver") ? "SILVER" : "BRONSE"
            };

            return rating;
        }

        private int GetPosition(string value)
        {
            value = value.Trim(' ', '+', '-');
            return string.IsNullOrEmpty(value) ? 0 : Convert.ToInt16(value);
        }
    }
}