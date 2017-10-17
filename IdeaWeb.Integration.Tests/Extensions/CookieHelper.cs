using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Net.Http.Headers;

namespace IdeaWeb.Integration.Tests.Extensions
{
    public static class CookieExtensions
    {
        /// <summary>
        /// Copies the cookies from a <see cref="HttpResponseMessage"/> into a new <see cref="HttpContent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void CopyCookiesFromResponse(this HttpContent request, HttpResponseMessage response) =>
            request.AddCookies(response.ExtractCookies());

        /// <summary>
        /// Extracts the cookies from a <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static IList<SetCookieHeaderValue> ExtractCookies(this HttpResponseMessage response) =>
            response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values) ?
                SetCookieHeaderValue.ParseList(values.ToList()) : null;

        /// <summary>
        /// Adds the given cookies to a <see cref="HttpContent"/> request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookies"></param>
        public static void AddCookies(this HttpContent request, IList<SetCookieHeaderValue> cookies) =>
            cookies.Where(c => c.Name.HasValue && c.Value.HasValue)
                .Select(c => new CookieHeaderValue(c.Name.Value, c.Value.Value).ToString())
                .ToList()
                .ForEach(c => request.Headers.Add("Cookie", c));
    }
}
