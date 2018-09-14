using System;
using System.Collections.Generic;
using System.Text;

namespace IBOTracker.Parsing
{
    internal static class Consts
    {
        public const int SchoolTableCellInRow = 6;
        public const string PageParameterName = "page";
        public const string PartialParameterName = "partial";
        public const string PartialPageParameterName = "partialPage";

        public static Uri IBOUri = new Uri("https://ibo.org");
        public static Uri SchoolSearchUri = new Uri(IBOUri, "programmes/find-an-ib-school/");
        public const string SchoolTablePath = "//*[@id=\"mainContent\"]/div[3]/div/div/table/tbody";

        public const string True = "true";
        public const string False = "false";
    }
}
