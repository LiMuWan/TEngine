using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TEngine
{
    /// <summary>
    /// 默认 JSON 函数集辅助器。
    /// </summary>
    public class DefaultJsonHelper : Utility.Json.IJsonHelper
    {
        /// <summary>
        /// 将对象序列化为 JSON 字符串。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns>序列化后的 JSON 字符串。</returns>
        public string ToJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">要反序列化的 JSON 字符串。</param>
        /// <returns>反序列化后的对象。</returns>
        public T ToObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为对象。
        /// </summary>
        /// <param name="objectType">对象类型。</param>
        /// <param name="json">要反序列化的 JSON 字符串。</param>
        /// <returns>反序列化后的对象。</returns>
        public object ToObject(System.Type objectType, string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, objectType);
        }
        
        public Dictionary<int, T> ParseDictionary<T>(string json) where T : class
        {
            var jsonObject = JObject.Parse(json);
            var dict = new Dictionary<int, T>();

            foreach (var kvp in jsonObject)
            {
                T item = JsonConvert.DeserializeObject<T>(kvp.Value.ToString());
                dict.Add(int.Parse(kvp.Key), item);
            }

            return dict;
        }

        public Dictionary<string, T> ParseStringDictionary<T>(string json) where T : class
        {
            var jsonObject = JObject.Parse(json);
            var dict = new Dictionary<string, T>();

            foreach (var kvp in jsonObject)
            {
                T item = JsonConvert.DeserializeObject<T>(kvp.Value.ToString());
                dict.Add(kvp.Key, item);
            }

            return dict;
        }
    }
}