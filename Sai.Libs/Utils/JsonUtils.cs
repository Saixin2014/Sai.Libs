using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public class JsonUtils
    {

        public static string SerializeObject(object obj)
        {
            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, jSetting);
            return json;
        }

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, (JsonSerializerSettings)null);
        }
    }
}
