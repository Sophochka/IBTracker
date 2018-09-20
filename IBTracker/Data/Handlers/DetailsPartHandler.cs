using System;
using System.Collections.Generic;
using System.Linq;
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
            Logger.BeginProgress(schools.Count);
            foreach(var school in schools)
            {
                Thread.Sleep(500);
                Logger.SetProgress(count++);
            }

            return Enumerable.Empty<DetailsPart>();
        }

        public int Link(IDictionary<int, SchoolInfo> schools, IDictionary<int, BasePart> parts)
        {
            var detailsParts = parts as IDictionary<int, DetailsPart>;
            if (detailsParts == null) return 0;
            
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