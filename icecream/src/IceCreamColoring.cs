using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace icecream
{
    public static class IceCreamColoring
    {
        private static ConsoleColor _fieldColor = ConsoleColor.DarkRed;
        private static ConsoleColor _valueColor = ConsoleColor.DarkCyan;

        public static IEnumerable<(ConsoleColor?, string)> ConvertJsonIntoList(string json,
            ConsoleColor? fieldColor = null, ConsoleColor? valueColor = null)
        {
            _fieldColor = fieldColor ?? _fieldColor;
            _valueColor = valueColor ?? _valueColor;

            var list = new List<(ConsoleColor?, string)>();
            ProcessJToken(JToken.ReadFrom(new JsonTextReader(new System.IO.StringReader(json))), list);
            return list;
        }

        private static void ProcessJToken(JToken token, ICollection<(ConsoleColor?, string)> list)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    list.Add((null, "{"));
                    foreach (var child in token.Children<JProperty>())
                    {
                        ProcessJToken(child, list);
                        if (child.Next != null)
                        {
                            list.Add((null, ", "));
                        }
                    }

                    list.Add((null, "}"));
                    break;
                case JTokenType.Array:
                    list.Add((null, "["));

                    var i = 0;
                    foreach (var child in token.Children())
                    {
                        ProcessJToken(child, list);
                        if (i < token.Children().Count() - 1)
                        {
                            list.Add((null, ", "));
                        }

                        i++;
                    }

                    list.Add((null, "]"));
                    break;
                case JTokenType.Property:
                    list.Add((_fieldColor, $"\"{(token as JProperty).Name}\""));
                    list.Add((null, ": "));
                    ProcessJToken((token as JProperty).Value, list);
                    break;
                case JTokenType.None:
                case JTokenType.Constructor:
                case JTokenType.Comment:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Undefined:
                case JTokenType.Date:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                default:
                    if (token.Type == JTokenType.String)
                    {
                        list.Add((_valueColor, $"\"{token}\""));
                        break;
                    }

                    if (token.Type == JTokenType.Null)
                    {
                        list.Add((_valueColor, "null"));
                        break;
                    }

                    if (token.Type == JTokenType.Boolean)
                    {
                        list.Add((_valueColor, token.ToString().ToLower()));
                        break;
                    }

                    list.Add((_valueColor, token.ToString()));
                    break;
            }
        }
    }
}