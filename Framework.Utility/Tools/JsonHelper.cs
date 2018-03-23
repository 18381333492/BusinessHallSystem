using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Framework.Utility.Tools
{
    /// <summary>
    /// 对json数据的操作的封装
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 将对象序列化成JSON字符串
        /// </summary>
        /// <param name="o">序列化的对象</param>
        /// <returns></returns>
        public static string ToJsonString(object o)
        {
            return JsonConvert.SerializeObject(o, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string JsonString)
        {
            var res = JsonConvert.DeserializeObject<T>(JsonString);
            return res;
        }
    }
}
