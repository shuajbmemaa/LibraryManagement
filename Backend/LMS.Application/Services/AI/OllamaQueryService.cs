using LMS.Application.DTO.AI;
using LMS.Domain.Entities;
using LMS.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

namespace LMS.Application.Services.AI
{
    public class OllamaQueryService : IOllamaQueryService
    {
        private readonly HttpClient _http;
        private readonly ApplicationDbContext _dbContext;
        private const string OllamaEndpoint = "http://localhost:11434/v1/chat/completions";

        private static readonly Dictionary<string, string> PropertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
      { "reading_status", "ReadingStatus" }, { "title", "Title" }, { "author", "Author" },
      { "genre", "Genre" }, { "user_id", "UserId" }, { "id", "Id" },
      { "created_at", "CreatedAt" }, { "is_deleted", "IsDeleted" }, { "username", "UserName" },
      { "name", "Name" }, { "email", "Email" }
    };

        private const string SystemPrompt = "You are the LMS Assistant, a helpful and witty library AI. \r\n\r\n### CORE DIRECTIVE:\r\n- You must ALWAYS respond in JSON.\r\n- If the user is chatting (greeting, joking, or asking non-database questions), set all query fields to null and put the response in Chat.Intro.\r\n- If the user is searching, fill out the Entity, Filter, and Select fields.\r\n\r\n### JSON SCHEMA:\r\n{\r\n  \"Entity\": \"Book\" | \"ApplicationUser\" | null,\r\n  \"Filter\": [{ \"Field\": \"string\", \"Operator\": \"string\", \"Value\": \"any\" }] | null,\r\n  \"OrderBy\": \"string\" | null,\r\n  \"OrderDirection\": \"asc\" | \"desc\" | null,\r\n  \"Select\": [\"string\"] | null,\r\n  \"Limit\": number | null,\r\n  \"Chat\": {\r\n    \"Intro\": \"string\",\r\n    \"ItemTemplate\": \"string\",\r\n    \"Outro\": \"string\"\r\n  }\r\n}\r\n\r\n### DATABASE WORLD:\r\n- **Book**: Title, Author, Genre, ReadingStatus (WantToRead, Reading, Completed)\r\n- **ApplicationUser**: Name, UserName, Email\r\n\r\n### BEHAVIOR EXAMPLES:\r\n\r\nUser: \"Hi! Who are you?\"\r\nJSON:\r\n{\r\n  \"Entity\": null,\r\n  \"Filter\": null,\r\n  \"Chat\": {\r\n    \"Intro\": \"I'm Gemini, your friendly neighborhood Library Assistant. I can find books, check user profiles, or just chat about literature! How can I help?\",\r\n    \"ItemTemplate\": \"\",\r\n    \"Outro\": \"\"\r\n  }\r\n}\r\n\r\nUser: \"I'm looking for some horror books.\"\r\nJSON:\r\n{\r\n  \"Entity\": \"Book\",\r\n  \"Filter\": [{ \"Field\": \"Genre\", \"Operator\": \"contains\", \"Value\": \"Horror\" }],\r\n  \"Chat\": {\r\n    \"Intro\": \"Spooky choice! I found these horror titles for you:\",\r\n    \"ItemTemplate\": \"- {{Title}} by {{Author}}\",\r\n    \"Outro\": \"Hope you have the lights on! Found {{count}} books.\"\r\n  }\r\n}\r\n\r\nUser: \"That's cool, thanks!\"\r\nJSON:\r\n{\r\n  \"Entity\": null,\r\n  \"Filter\": null,\r\n  \"Chat\": {\r\n    \"Intro\": \"You're very welcome! Let me know if you need anything else.\",\r\n    \"ItemTemplate\": \"\",\r\n    \"Outro\": \"\"\r\n  }\r\n}";

        public OllamaQueryService(HttpClient httpClient, ApplicationDbContext dbContext)
        {
            _http = httpClient;
            _dbContext = dbContext;
        }

        public async Task<QueryPlan?> GenerateQueryPlanAsync(string question)
        {
            var body = new
            {
                model = "gemma3:12b",
                messages = new[] { new { role = "system", content = SystemPrompt }, new { role = "user", content = question } },
                stream = false,
                options = new { num_predict = 256, temperature = 0.3 }
            };

            var response = await _http.PostAsJsonAsync(OllamaEndpoint, body);
            response.EnsureSuccessStatusCode();

            var rawContent = await response.Content.ReadAsStringAsync();
            return ParseOllamaResponse(rawContent);
        }

        public async Task<(List<object> Data, int Count)> ExecuteQueryPlanAsync(QueryPlan plan)
        {
            if (string.IsNullOrEmpty(plan.Entity))
            {
                return (new List<object>(), 0);
            }

            IQueryable queryable = MapEntityToSet(plan.Entity);

            if (plan.Filter?.Any() == true)
            {
                queryable = ApplyFilters(queryable, plan.Filter);
            }

            if (!string.IsNullOrWhiteSpace(plan.OrderBy))
            {
                queryable = ApplyOrdering(queryable, plan.OrderBy, plan.OrderDirection);
            }

            if (plan.Limit.HasValue)
                queryable = queryable.Take(plan.Limit.Value);

            if (plan.Select?.Any() == true)
            {
                var fields = string.Join(",", plan.Select);
                queryable = queryable.Select($"new({fields})");
            }

            var list = await queryable.Cast<object>().ToListAsync();
            return (list, list.Count);
        }

        #region Helper Methods

        private QueryPlan? ParseOllamaResponse(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                if (string.IsNullOrWhiteSpace(content)) return null;

                var cleanedJson = content.Replace("```json", "").Replace("```", "").Trim();
                var raw = JsonSerializer.Deserialize<RawQueryPlan>(cleanedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (raw == null) return null;

                return new QueryPlan
                {
                    Entity = raw.Entity ?? "",
                    OrderBy = raw.OrderBy,
                    OrderDirection = raw.OrderDirection,
                    Chat = raw.Chat ?? new ChatMessagePlan(),
                    Limit = ParseLimit(raw.Limit),
                    Select = ParseSelect(raw.Select),
                    Filter = ParseFilters(raw.Filter)
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ollama Parsing Error: {ex.Message}");
                return null;
            }
        }

        private IQueryable MapEntityToSet(string entityName)
        {
            return entityName.ToLower() switch
            {
                "book" or "books" => _dbContext.Set<Domain.Entities.Book>().AsNoTracking(),
                "applicationuser" or "user" => _dbContext.Set<ApplicationUser>().AsNoTracking(),
                _ => throw new ArgumentException($"Entity '{entityName}' is not supported.")
            };
        }

        private List<string>? ParseSelect(object? selectObj)
        {
            if (selectObj is not JsonElement element) return null;

            return element.ValueKind switch
            {
                JsonValueKind.Array => element.EnumerateArray()
                  .Where(x => x.ValueKind == JsonValueKind.String)
                  .Select(x => x.GetString()!)
                  .ToList(),

                JsonValueKind.String => element.GetString()!
                  .Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(x => x.Trim())
                  .ToList(),

                _ => null
            };
        }

        private int? ParseLimit(object? limitObj)
        {
            if (limitObj is not JsonElement element) return null;

            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var val))
                return val;

            if (element.ValueKind == JsonValueKind.String && int.TryParse(element.GetString(), out var strVal))
                return strVal;

            return null;
        }

        private List<QueryFilter> ParseFilters(object? filterObj)
        {
            var list = new List<QueryFilter>();
            if (filterObj is not JsonElement element || element.ValueKind != JsonValueKind.Array)
                return list;

            foreach (var item in element.EnumerateArray())
            {
                try
                {
                    var filter = JsonSerializer.Deserialize<QueryFilter>(item.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (filter != null && !string.IsNullOrWhiteSpace(filter.Field))
                        list.Add(filter);
                }
                catch
                {
                    continue;
                }
            }
            return list;
        }

        private IQueryable ApplyFilters(IQueryable query, List<QueryFilter> filters)
        {
            var filterParts = new List<string>();
            var values = new List<object>();

            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];
                var propName = PropertyMap.GetValueOrDefault(filter.Field, filter.Field);
                var property = query.ElementType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (property == null) continue;

                var val = ConvertJsonValue(filter.Value, property.PropertyType);
                if (val == null) continue;

                string op = filter.Operator.ToLower();
                string clause = property.PropertyType == typeof(string)
                  ? GetStringClause(propName, op, i)
                  : GetValueClause(propName, op, i);

                filterParts.Add(clause);
                values.Add(property.PropertyType == typeof(string) ? val.ToString()!.ToLower() : val);
            }

            return filterParts.Any()
              ? query.Where(string.Join(" && ", filterParts), values.ToArray())
              : query;
        }

        private string GetStringClause(string prop, string op, int idx) => op switch
        {
            "contains" => $"{prop}.ToLower().Contains(@{idx})",
            "startswith" => $"{prop}.ToLower().StartsWith(@{idx})",
            "endswith" => $"{prop}.ToLower().EndsWith(@{idx})",
            _ => $"{prop}.ToLower() == @{idx}"
        };

        private string GetValueClause(string prop, string op, int idx) => op switch
        {
            "gt" => $"{prop} > @{idx}",
            "lt" => $"{prop} < @{idx}",
            "gte" => $"{prop} >= @{idx}",
            "lte" => $"{prop} <= @{idx}",
            _ => $"{prop} == @{idx}"
        };

        private object? ConvertJsonValue(object? rawValue, Type targetType)
        {
            if (rawValue is not JsonElement json) return rawValue;

            return json.ValueKind switch
            {
                JsonValueKind.String => targetType == typeof(Guid) ? Guid.Parse(json.GetString()!)
                           : targetType.IsEnum ? Enum.Parse(targetType, json.GetString()!, true)
                           : json.GetString(),
                JsonValueKind.Number => json.GetInt32(),
                JsonValueKind.True or JsonValueKind.False => json.GetBoolean(),
                _ => null
            };
        }

        private IQueryable ApplyOrdering(IQueryable query, string field, string? direction)
        {
            var propName = PropertyMap.GetValueOrDefault(field, field);
            bool isDescending = direction?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
            return query.OrderBy($"{propName} {(isDescending ? "descending" : "ascending")}");
        }

        #endregion
    }
}