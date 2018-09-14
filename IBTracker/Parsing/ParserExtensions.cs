using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace IBOTracker.Parsing
{
    public static class ParserExtensions
    {
        public static void ParsePages(Func<int, Uri> pagesFunc, Func<HtmlDocument, bool> parseFunc)
        {
            var page = 1;
            var completed = false;
            var webClient = new WebClient();
            do
            {
                var url = pagesFunc(page++);
                var document = new HtmlDocument();
                document.Load(webClient.OpenRead(url), Encoding.UTF8);
                completed = parseFunc(document);
            } while (!completed);
        }

        public static IEnumerable<T> ParseTable<T>(this HtmlDocument document, string path, Func<HtmlNodeCollection, T> rowFunc)
        {
            var table = document.DocumentNode.SelectNodes(path).FirstOrDefault();
            if (table == null) return Enumerable.Empty<T>();

            var rows = table.SelectNodes(HTML.TR);
            if (rows == null) return Enumerable.Empty<T>();

            return rows.Select(r => rowFunc(r.SelectNodes(HTML.TD))).Where(i => i != null);
        }

        public static bool ParseLink(this HtmlNode node, out string text, out string href)
        {
            text = null;
            href = null;
            var link = node.SelectNodes(HTML.A).FirstOrDefault();
            if (link == null) return false;
            text = link.InnerText;
            href = link.GetAttributeValue(HTML.HREF, null);
            return true;
        }

        public static bool ParseCheck(this HtmlNode node, string path)
        {
            return node.SelectNodes(path)?.FirstOrDefault() != null;
        }
    }
}
