using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RanaUtils.IO
{
    public static class FileIO
    {
        public static void ParseTextFile(string filePath, Dictionary<string, string> replacements)
        {
            string text = File.ReadAllText(filePath);
            int index = 0;

            while(index < text.Length)
            {
                // Skip blank lines / whitespace between entries
                while(index < text.Length &&
                       char.IsWhiteSpace(text[index]) && text[index] != '\r' && text[index] != '\n')
                {
                    index++;
                }

                while(index < text.Length && (text[index] == '\r' || text[index] == '\n'))
                {
                    index++;
                }

                if(index >= text.Length)
                {
                    break;
                }

                // Read key
                int keyStart = index;
                while(index < text.Length && text[index] != '=' && text[index] != '\r' && text[index] != '\n')
                {
                    index++;
                }

                if(index >= text.Length || text[index] != '=')
                {
                    while(index < text.Length && text[index] != '\r' && text[index] != '\n')
                    {
                        index++;
                    }

                    continue;
                }

                string key = text.Substring(keyStart, index - keyStart).Trim();
                index++; // skip '='

                // Skip spaces before value
                while(index < text.Length &&
                       char.IsWhiteSpace(text[index]) && text[index] != '\r' && text[index] != '\n')
                {
                    index++;
                }

                string value = index < text.Length && text[index] == '"' ? ReadQuotedValue(text, ref index) : ReadUnquotedValue(text, ref index);
                replacements[key] = value;
            }
        }

        private static string ReadQuotedValue(string text, ref int index)
        {
            StringBuilder builder = new();
            index++; // skip opening quote

            while(index < text.Length)
            {
                // Handle escaped sequences like \n, \r, \t, \", \\
                if(text[index] == '\\' && index + 1 < text.Length)
                {
                    _=text[index + 1] switch
                    {
                        'n' => builder.Append('\n'),
                        'r' => builder.Append('\r'),
                        't' => builder.Append('\t'),
                        '"' => builder.Append('"'),
                        '\\' => builder.Append('\\'),
                        _ => builder.Append(text[index + 1]),
                    };
                    index += 2;
                    continue;
                }

                if(text[index] == '"')
                {
                    index++; // consume closing quote
                    break;
                }

                _=builder.Append(text[index]);
                index++;
            }

            // Ignore trailing comments after quoted value
            while(index < text.Length && text[index] != '\r' && text[index] != '\n')
            {
                if(text[index] == '/' && index + 1 < text.Length && text[index + 1] == '/')
                {
                    index += 2;
                    while(index < text.Length && text[index] != '\r' && text[index] != '\n')
                    {
                        index++;
                    }

                    break;
                }

                index++;
            }

            while(index < text.Length && (text[index] == '\r' || text[index] == '\n'))
            {
                index++;
            }

            return builder.ToString();
        }

        private static string ReadUnquotedValue(string text, ref int index)
        {
            StringBuilder builder = new();

            while(index < text.Length && text[index] != '\r' && text[index] != '\n')
            {
                if(text[index] == '/' && index + 1 < text.Length && text[index + 1] == '/')
                {
                    break;
                }

                _=builder.Append(text[index]);
                index++;
            }

            while(index < text.Length && (text[index] == '\r' || text[index] == '\n'))
            {
                index++;
            }

            return builder.ToString().Trim();
        }

        public static void WriteTextFile(string filePath, Dictionary<string, string> replacements)
        {
            using StreamWriter stream = new(filePath);

            foreach(KeyValuePair<string, string> kvp in replacements)
            {
                string value = kvp.Value ?? string.Empty;

                if(value.Contains('\r') ||
                    value.Contains('\n') ||
                    value.Contains('"') ||
                    value.Contains("//") ||
                    value.StartsWith(" ") ||
                    value.EndsWith(" "))
                {
                    value = value
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\r", "\\r")
                        .Replace("\n", "\\n");

                    stream.WriteLine($"{kvp.Key}=\"{value}\"");
                }
                else
                {
                    stream.WriteLine($"{kvp.Key}={value}");
                }
            }
        }
    }
}
