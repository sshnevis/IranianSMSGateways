using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
                //jsonValue = JsonEscape(jsonValue);


                //string JsonEscape(string s) =>
                //    s.Replace("\\", "\\\\")
                //     .Replace("\"", "\\\"")
                //     .Replace("\n", "\\n")
                //     .Replace("\r", "\\r");

                jsonParts.Add($"\"{name}\":{jsonValue}");
            }

            return "{" + string.Join(",", jsonParts) + "}";
        }


        public static string JsonEscape(string s)
        {
            return s.Replace("\\", "\\\\")
             .Replace("\"", "\\\"")
             .Replace("\n", "\\n")
             .Replace("\r", "\\r");
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

    public static class JsonConvert2
    {


        // ======================================================
        // PUBLIC API
        // ======================================================

        public static T DeserializeObject<T>(string json) where T : new()
        {
            int index = 0;
            var parsed = ParseValue(json.Trim(), ref index);
            return (T)ConvertTo(parsed, typeof(T));
        }

        public static string SerializeObject(object obj)
        {
            return SerializeValue(obj);
        }

        // ======================================================
        // JSON → .NET PARSER
        // (نسخهٔ قبلی + یک مقدار بهینه‌سازی)
        // ======================================================

        private static object ParseValue(string json, ref int index)
        {
            SkipWhitespace(json, ref index);

            if (json[index] == '{') return ParseObject(json, ref index);
            if (json[index] == '[') return ParseArray(json, ref index);
            if (json[index] == '"') return ParseString(json, ref index);

            return ParseLiteral(json, ref index);
        }

        private static Dictionary<string, object> ParseObject(string json, ref int index)
        {
            var obj = new Dictionary<string, object>();
            index++; // skip {

            while (true)
            {
                SkipWhitespace(json, ref index);
                if (json[index] == '}')
                {
                    index++;
                    break;
                }

                string key = ParseString(json, ref index);

                SkipWhitespace(json, ref index);
                index++; // :

                object value = ParseValue(json, ref index);
                obj[key] = value;

                SkipWhitespace(json, ref index);

                if (json[index] == ',')
                {
                    index++;
                    continue;
                }

                if (json[index] == '}')
                {
                    index++;
                    break;
                }
            }

            return obj;
        }

        private static List<object> ParseArray(string json, ref int index)
        {
            var list = new List<object>();
            index++; // [

            while (true)
            {
                SkipWhitespace(json, ref index);

                if (json[index] == ']')
                {
                    index++;
                    break;
                }

                var val = ParseValue(json, ref index);
                list.Add(val);

                SkipWhitespace(json, ref index);

                if (json[index] == ',')
                {
                    index++;
                    continue;
                }
                if (json[index] == ']')
                {
                    index++;
                    break;
                }
            }

            return list;
        }

        private static string ParseString(string json, ref int index)
        {
            index++; // skip "
            var sb = new StringBuilder();

            while (json[index] != '"')
            {
                sb.Append(json[index]);
                index++;
            }

            index++; // closing "
            return sb.ToString();
        }

        private static object ParseLiteral(string json, ref int index)
        {
            int start = index;

            while (index < json.Length && " ,]}".IndexOf(json[index]) == -1)
                index++;

            string token = json.Substring(start, index - start);

            if (int.TryParse(token, out int i)) return i;
            if (long.TryParse(token, out long l)) return l;
            if (double.TryParse(token, out double d)) return d;
            if (bool.TryParse(token, out bool b)) return b;
            if (token == "null") return null;

            return token;
        }

        private static void SkipWhitespace(string json, ref int index)
        {
            while (index < json.Length && char.IsWhiteSpace(json[index]))
                index++;
        }

        // ======================================================
        // JSON Parser → Strong Type
        // ======================================================

        private static object ConvertTo(object value, Type targetType)
        {
            if (value == null)
                return null;

            if (targetType.IsAssignableFrom(value.GetType()))
                return value;

            if (value is string s && targetType != typeof(string))
                return Convert.ChangeType(s, targetType);

            if (value is IConvertible && targetType.IsPrimitive)
                return Convert.ChangeType(value, targetType);

            // LIST<T>
            if (typeof(IEnumerable).IsAssignableFrom(targetType) &&
                targetType.IsGenericType)
            {
                var list = (IList)Activator.CreateInstance(targetType);
                var elementType = targetType.GetGenericArguments()[0];

                foreach (var item in (List<object>)value)
                    list.Add(ConvertTo(item, elementType));

                return list;
            }

            // OBJECT
            if (value is Dictionary<string, object> dict)
            {
                var obj = Activator.CreateInstance(targetType);

                foreach (var prop in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!prop.CanWrite) continue;
                    foreach (var key in dict.Keys)
                    {
                        if (string.Equals(prop.Name, key, StringComparison.OrdinalIgnoreCase))
                        {
                            var converted = ConvertTo(dict[key], prop.PropertyType);
                            prop.SetValue(obj, converted);
                        }
                    }
                }
                return obj;
            }

            return value;
        }

        // ======================================================
        // .NET → JSON (Serializer)
        // ======================================================

        private static string SerializeValue(object obj)
        {
            if (obj == null) return "null";

            Type t = obj.GetType();

            // string
            if (obj is string s)
                return "\"" + EscapeString(s) + "\"";

            // bool
            if (obj is bool b)
                return b ? "true" : "false";

            // numbers
            if (obj is int || obj is long || obj is double || obj is float || obj is decimal)
                return obj.ToString();

            // list / array
            if (obj is IEnumerable enumerable && !(obj is string))
                return SerializeArray(enumerable);

            // object
            return SerializeObjectInternal(obj);
        }

        private static string SerializeArray(IEnumerable list)
        {
            var json = new StringBuilder();
            json.Append("[");

            bool first = true;
            foreach (var item in list)
            {
                if (!first) json.Append(",");
                json.Append(SerializeValue(item));
                first = false;
            }

            json.Append("]");
            return json.ToString();
        }

        private static string SerializeObjectInternal(object obj)
        {
            var json = new StringBuilder();
            json.Append("{");

            bool first = true;
            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (!prop.CanRead) continue;

                var value = prop.GetValue(obj);
                if (!first) json.Append(",");

                json.Append("\"");
                json.Append(prop.Name);
                json.Append("\":");
                json.Append(SerializeValue(value));

                first = false;
            }

            json.Append("}");
            return json.ToString();
        }

        private static string EscapeString(string s)
        {
            return s
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }



    }

}