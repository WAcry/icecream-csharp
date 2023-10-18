using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Linq;

namespace icecream
{
    public class Coloring
    {
        public static IEnumerable<(ConsoleColor?, string)> ConvertJsonIntoList(string json)
        {
            var list = new List<(ConsoleColor?, string)>();

            ProcessJToken(JToken.Parse(json), list);

            return list;
        }

        private static void ProcessJToken(JToken token, IList<(ConsoleColor?, string)> list)
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
                    list.Add((ConsoleColor.DarkRed, $"\"{(token as JProperty).Name}\""));
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
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                default:
                    if (token.Type == JTokenType.String)
                    {
                        list.Add((ConsoleColor.DarkCyan, $"\"{token}\""));
                        break;
                    }

                    if (token.Type == JTokenType.Null)
                    {
                        list.Add((ConsoleColor.DarkCyan, "null"));
                        break;
                    }

                    list.Add((ConsoleColor.DarkCyan, token.ToString()));
                    break;
            }
        }
    }
}