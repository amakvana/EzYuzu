using System;
using System.Net;

namespace EzYuzu
{
    public static class YuzuUtilities
    {
        /// <summary>
        /// Gets a redirected URL from any given URL 
        /// </summary>
        /// <param name="url">The original URL</param>
        /// <returns>The redirected URL</returns>
        // https://stackoverflow.com/a/24445779
        public static string GetRedirectedUrl(string url)
        {
            string uriString = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = false;  // IMPORTANT
            request.Timeout = 10000;           // timeout 10s
            request.Method = "HEAD";
            // Get the response ...
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // Now look to see if it's a redirect
                if ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399)
                {
                    uriString = response.Headers["Location"];
                    response.Close(); // don't forget to close it - or bad things happen
                }
            }
            return uriString;
        }

        /// <summary>
        /// Fetches the latest version number of Yuzu online
        /// </summary>
        /// <returns>The version number</returns>
        public static int FetchLatestYuzuVersionNumber(string url)
        {
            if (url.EndsWith("/latest", StringComparison.Ordinal))
            {
                url = GetRedirectedUrl($"{url}");
            }
            string version = url.Substring(url.LastIndexOf('/') + 1).Trim();
            version = version.Substring(version.LastIndexOf('-') + 1).Trim();
            return int.Parse(version);
        }
    }
}
