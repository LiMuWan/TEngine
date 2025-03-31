using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic
{

    public static class HttpCode
    {
        public const int Success = 0;
    }

    public class Resp
    {
        public int code;
        public string msg;
    }

    public class Resp<T> : Resp
    {
        public T data;
    }

    public class Req
    {
        private string url;
        private Dictionary<string, string> param;
        public Dictionary<string, string> headers { get; private set; }

        public static Req Gen(string url)
        {
            Req req = new Req { url = url };
            return req;
        }

        public void AddParam(string key, string value)
        {
            param ??= new Dictionary<string, string>();
            param[key] = value;
        }

        public void AddHeader(string key, string value)
        {
            headers ??= new Dictionary<string, string>();
            headers[key] = value;
        }

        public Uri Uri()
        {
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append(url);
            if (param != null && param.Count > 0)
            {
                urlBuilder.Append("?");
                int index = 0;
                foreach (KeyValuePair<string, string> kvp in param)
                {
                    if (index > 0)
                    {
                        urlBuilder.Append("&");
                    }

                    urlBuilder.Append($"{kvp.Key}={kvp.Value}");
                    index++;
                }
            }

            return new Uri(urlBuilder.ToString());
        }
    }
}