using LMS.Application.DTO.AI;
using System.Reflection;
using System.Text;

namespace LMS.Application.Services.AI
{
    public static class ChatResponseBuilder
    {
        public static string Build(
            ChatMessagePlan chat,
            IEnumerable<object> data,
            int count,
            int maxItems = 5)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(chat.Intro))
                sb.AppendLine(chat.Intro.Trim());

            var template = chat.ItemTemplate ?? "";
            bool hasPlaceholders = template.Contains("{{") && template.Contains("}}");

            if (!hasPlaceholders && !string.IsNullOrWhiteSpace(template))
            {
                sb.AppendLine(template.Trim());
            }
            else if (hasPlaceholders)
            {
                var items = data.Take(maxItems).ToList();
                foreach (var item in items)
                {
                    var line = template;
                    var props = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var prop in props)
                    {
                        if (prop.GetIndexParameters().Length > 0) continue;

                        string placeholder = $"{{{{{prop.Name}}}}}";

                        if (line.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
                        {
                            object? val = null;
                            try { val = prop.GetValue(item); } catch { }

                            line = ReplaceCaseInsensitive(line, placeholder, val?.ToString() ?? "");
                        }
                    }
                    sb.AppendLine(line.Trim());
                }
            }

            if (!string.IsNullOrWhiteSpace(chat.Outro))
            {
                var outro = chat.Outro.Replace("{{count}}", count.ToString(), StringComparison.OrdinalIgnoreCase);
                sb.AppendLine(outro.Trim());
            }

            return sb.ToString();
        }

        private static string ReplaceCaseInsensitive(string str, string placeholder, string newValue)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                str,
                System.Text.RegularExpressions.Regex.Escape(placeholder),
                newValue,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}
