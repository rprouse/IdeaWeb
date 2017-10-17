using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace IdeaWeb.Integration.Tests.Data
{
    public static class CookieHelper
    {
        public static void CopyCookiesFromResponse(this HttpContent request, HttpResponseMessage response) =>
            request.AddCookies(response.ExtractCookies());

        public static IList<SetCookieHeaderValue> ExtractCookies(this HttpResponseMessage response) =>
            response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values) ?
                SetCookieHeaderValue.ParseList(values.ToList()) : null;

        public static void AddCookies(this HttpContent request, IList<SetCookieHeaderValue> cookies) =>
            cookies.Where(c => c.Name.HasValue && c.Value.HasValue)
                .Select(c => new CookieHeaderValue(c.Name.Value, c.Value.Value).ToString())
                .ToList()
                .ForEach(c => request.Headers.Add("Cookie", c));
    }
}
