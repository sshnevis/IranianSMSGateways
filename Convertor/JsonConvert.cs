using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IranianSMSGateways.Convertor
{
    public static class JsonConvert
    {
        public static string SerializeObject(object obj)
        {
            var jsonParts = new List<string>();
            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                string name = prop.Name;
                object value = prop.GetValue(obj, null);

                string jsonValue = value is string
                    ? $"\"{value}\""
                    : value is bool
                        ? value.ToString().ToLower()
                        : value?.ToString();

                jsonParts.Add($"\"{name}\":{jsonValue}");
            }

            return "{" + string.Join(",", jsonParts) + "}";
        }

        public static string ListToJson(List<string> items)
        {
            if (items == null || items.Count == 0)
                return "[]";

            var quoted = items.Select(x => $"\"{x}\"");
            return "[" + string.Join(",", quoted) + "]";
        }


        public static List<string> DeserializeJsonArray(string json)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(json))
                return result;

            json = json.Trim().TrimStart('[').TrimEnd(']');

            var items = json.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                var cleaned = item.Trim().Trim('"');
                result.Add(cleaned);
            }

            return result;
        }

        public static T DeserializeObject<T>(string json) where T : new()
        {
            var obj = new T();
            var type = typeof(T);
            json = json.Trim('{', '}');
            var pairs = json.Split(',');

            foreach (var pair in pairs)
            {
                var kv = pair.Split(':');
                if (kv.Length != 2) continue;

                string key = kv[0].Trim('\"', ' ');
                string value = kv[1].Trim('\"', ' ');

                var prop = type.GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null && prop.CanWrite)
                {
                    object convertedValue = Convert.ChangeType(value, prop.PropertyType);
                    prop.SetValue(obj, convertedValue);
                }
            }

            return obj;
        }


    }
}
