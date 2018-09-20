using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using IBTracker.Contracts;
using IBTracker.Utils;

namespace IBTracker.Data
{
    public class DetailsPartHandler : IPartHandler
    {
        public IEnumerable<BasePart> Read(ICollection<SchoolInfo> schools)
        {
            return null;
        }

        public int Link(ICollection<SchoolInfo> schools, IEnumerable<PartLink> links, IEnumerable<BasePart> parts)
        {
            return 0;
        }

        private DetailsPart ParsePage(HtmlNodeCollection rowCells)
        {
            // https://www.google.com/maps/?q=51.2499,22.5357
            var detailsPart = new DetailsPart
            {
                Name = "",
                City = "",
            };

            return detailsPart;
        }
    }
}