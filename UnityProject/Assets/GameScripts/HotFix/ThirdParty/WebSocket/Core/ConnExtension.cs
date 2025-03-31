using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace GameBase
{
    public static class ConnExtension
    {
        public static string AddParams(string url, Dictionary<string, string> paramsDic)
        {
            // combine url
            if (paramsDic != null)
            {
                UriBuilder uriBuilder = new UriBuilder(url);
                Dictionary<string, string> queryDict = ParseQueryString(uriBuilder.Query);
                foreach (KeyValuePair<string, string> pair in paramsDic)
                {
                    queryDict[pair.Key] = pair.Value;
                }

                uriBuilder.Query = queryDict.ToQueryString();
                url = uriBuilder.ToString();
            }

            return url;
        }

        public static Dictionary<string, string> ParseQueryString(string query)
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            foreach (var item in query.TrimStart('?').Split('&'))
            {
                var parts = item.Split('=');
                if (parts.Length >= 2)
                {
                    string key = UnityWebRequest.UnEscapeURL(parts[0]);
                    string value = UnityWebRequest.UnEscapeURL(parts[1]);
                    collection.Add(key, value);
                }
            }

            return collection;
        }

        public static string ToQueryString(this Dictionary<string, string> queryDict)
        {
            return string.Join("&", queryDict.Select(x => $"{UnityWebRequest.EscapeURL(x.Key)}={UnityWebRequest.EscapeURL(x.Value)}"));
        }
    }
}