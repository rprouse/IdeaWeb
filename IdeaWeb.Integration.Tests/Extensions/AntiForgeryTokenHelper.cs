using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace IdeaWeb.Integration.Tests.Extensions
{
    public static class AntiForgeryTokenExtensions
    {
        /// <summary>
        /// Extracts an anti-forgery token from an HTML form
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<string> ExtractAntiForgeryToken(this HttpResponseMessage response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            string html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.ExtractAntiForgeryToken();
        }

        /// <summary>
        /// Extracts an anti-forgery token from an HTML form
        /// </summary>
        /// <param name="htmlDocument"></param>
        /// <returns></returns>
        public static string ExtractAntiForgeryToken(this HtmlDocument htmlDocument)
        {
            if (htmlDocument == null)
                throw new ArgumentNullException(nameof(htmlDocument));

            return htmlDocument.DocumentNode
                               .Descendants("input")
                               .Where(i => i.Attributes["name"]?.Value == "__RequestVerificationToken")
                               .Select(i => i.Attributes["value"]?.Value)
                               .FirstOrDefault();
        }
    }
}
