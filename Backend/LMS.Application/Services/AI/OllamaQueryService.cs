using LMS.Application.DTO.AI;
using LMS.Domain.Entities;
using LMS.Infrastructure;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

namespace LMS.Application.Services.AI
{
    public class OllamaQueryService : IOllamaQueryService
    {
        private readonly HttpClient _http;
        private readonly ApplicationDbContext _dbContext;
        private const string Url = "http://localhost:11434/v1/chat/completions";

        private const string SystemPrompt = @"
You are a Query Planner AI. 
Your job is to convert natural language questions into a VALID QueryPlan JSON used by a .NET API.

You MUST follow these rules strictly:

1) Always output ONLY valid JSON. No text, no explanations, no markdown.
2) The JSON structure MUST be:

{
  ""Entity"": ""..."",
  ""Filter"": [],
  ""OrderBy"": null,
  ""OrderDirection"": null,
  ""Select"": null,
  ""Limit"": null
}

3) Entity MUST be one of:
- ""ApplicationUser""
- ""Book""

4) Filter MUST be a JSON array of objects:
[
  {
    ""Field"": ""PropertyName"",
    ""Operator"": ""eq|contains|startswith|endswith|gt|lt|gte|lte|in"",
    ""Value"": ""something""
  }
]

If no filters are needed → return an empty array: []

5) OrderBy MUST be a REAL property name of the entity.
NEVER set OrderBy to ""asc"", ""desc"", ""+UserName"", or anything invalid.

If user does not request sorting → OrderBy = null and OrderDirection = null.

6) Select must be either:
- null → return full entity
- Array of existing property names

7) Limit must be either:
- null
- number (example: 10)

8) NEVER guess fields that don't exist.
Allowed ApplicationUser fields:
- Id, UserName, Name, Email, CreatedAt, IsDeleted

Allowed Book fields:
- Id, Title, Author, Genre, ReadingStatus, UserId, CreatedBy, CreatedAt, IsDeleted

9) ONLY return JSON. No markdown. No code blocks.

EXAMPLES:

User: ""show me all users""
Return:
{
  ""Entity"": ""ApplicationUser"",
  ""Filter"": [],
  ""OrderBy"": null,
  ""OrderDirection"": null,
  ""Select"": null,
  ""Limit"": null
}

User: ""all books with reading status reading""
Return:
{
  ""Entity"": ""Book"",
  ""Filter"": [
    { ""Field"": ""ReadingStatus"", ""Operator"": ""eq"", ""Value"": ""Reading"" }
  ],
  ""OrderBy"": null,
  ""OrderDirection"": null,
  ""Select"": null,
  ""Limit"": null
}
";


        public OllamaQueryService(HttpClient httpClient, ApplicationDbContext dbContext)
        {
            _http = httpClient;
            _dbContext = dbContext;
        }

        public async Task<QueryPlan?> GenerateQueryPlanAsync(string question)
        {
            var body = new
            {
                model = "gemma3:1b",
                messages = new[]
                {
            new { role = "system", content = SystemPrompt },
            new { role = "user", content = question }
        },
                stream = false
            };

            var response = await _http.PostAsJsonAsync(Url, body);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                using var doc = JsonDocument.Parse(rawResponse);
                var root = doc.RootElement;

                if (!root.TryGetProperty("choices", out var choices) ||
                    choices.ValueKind != JsonValueKind.Array ||
                    !choices.EnumerateArray().Any())
                {
                    return null;
                }

                var content = choices
                    .EnumerateArray()
                    .First()
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                if (string.IsNullOrWhiteSpace(content))
                    return null;

                var cleanedContent = content
                    .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("```", "")
                    .Trim();

                var plan = JsonSerializer.Deserialize<QueryPlan>(cleanedContent, options);

                if (plan == null)
                    return null;

                plan.Filter ??= new List<QueryFilter>();

                return plan;
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Ollama JSON parse error:");
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<string> FormatResultAsync(string question, string entity, object data, int count)
            {
            var prompt = $"""
You are an assistant inside a Library Management System.

User question:
"{question}"

Entity: {entity}
Total results count: {count}

Database result (JSON):
{JsonSerializer.Serialize(data)}

Rules:
- There ARE results (count > 0)
- DO NOT say "No results found"
- Answer in clear, short, human-friendly sentences
- Summarize the list
- Mention the total count
- For books: show Title, Author, ReadingStatus
- For users: show Name and Email
""";

            var body = new
            {
                model = "gemma3:1b",
                messages = new[]
                {
            new { role = "user", content = prompt }
        }
            };

            var res = await _http.PostAsJsonAsync(Url, body);
            res.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";
        }


        public async Task<object> ExecuteQueryPlanAsync(QueryPlan plan)
        {
            IQueryable queryable;

            var entityMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "User", "ApplicationUser" },
                { "ApplicationUser", "ApplicationUser" },
                { "Book", "Book" },
                { "Books", "Book" }
            };

            if (!entityMap.TryGetValue(plan.Entity, out var mappedEntity))
                throw new ArgumentException($"Entity {plan.Entity} not supported.");

            queryable = mappedEntity switch
            {
                "ApplicationUser" => _dbContext.Set<ApplicationUser>(),
                "Book" => _dbContext.Set<Domain.Entities.Book>(),
                _ => throw new ArgumentException($"Entity {plan.Entity} not supported.")
            };

            var propertyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "reading_status", "ReadingStatus" },
                { "title", "Title" },
                { "author", "Author" },
                { "genre", "Genre" },
                { "user_id", "UserId" },
                { "id", "Id" },
                { "created_at", "CreatedAt" },
                { "created_by", "CreatedBy" },
                { "is_deleted", "IsDeleted" },

                { "username", "UserName" },
                { "name", "Name" },
                { "email", "Email" },
                { "created_at_user", "CreatedAt" },
                { "is_deleted_user", "IsDeleted" }
            };

            if (plan.Filter != null && plan.Filter.Any())
            {
                var filterParts = new List<string>();
                var values = new List<object>();
                int index = 0;

                foreach (var filter in plan.Filter)
                {
                    var propName = propertyMap.ContainsKey(filter.Field) ? propertyMap[filter.Field] : filter.Field;
                    var property = queryable.ElementType.GetProperty(propName);

                    if (property == null)
                    {
                        Console.WriteLine($"Warning: Property '{filter.Field}' does not exist on entity '{mappedEntity}'");
                        continue;
                    }

                    object val;

                    if (filter.Value is JsonElement jsonVal)
                    {
                        if (property.PropertyType == typeof(string))
                            val = jsonVal.GetString() ?? "";
                        else if (property.PropertyType.IsEnum)
                            val = Enum.Parse(property.PropertyType, jsonVal.GetString() ?? "");
                        else if (property.PropertyType == typeof(Guid))
                            val = Guid.Parse(jsonVal.GetString() ?? "");
                        else if (property.PropertyType == typeof(int))
                            val = jsonVal.GetInt32();
                        else if (property.PropertyType == typeof(bool))
                            val = jsonVal.GetBoolean();
                        else if (property.PropertyType == typeof(DateTime))
                            val = jsonVal.GetDateTime();
                        else
                            val = jsonVal.GetRawText();
                    }
                    else
                    {
                        val = filter.Value?.ToString() ?? "";
                    }

                    if (val == null || (property.PropertyType == typeof(string) && string.IsNullOrWhiteSpace(val.ToString())))
                        continue;

                    string clause = "";

                    if (property.PropertyType == typeof(string))
                    {
                        val = val.ToString().ToLower();
                        clause = filter.Operator.ToLower() switch
                        {
                            "eq" => $"({propName} != null && {propName}.ToLower() == @{index})",
                            "contains" => $"({propName} != null && {propName}.ToLower().Contains(@{index}))",
                            "startswith" => $"({propName} != null && {propName}.ToLower().StartsWith(@{index}))",
                            "endswith" => $"({propName} != null && {propName}.ToLower().EndsWith(@{index}))",
                            _ => $"({propName} != null && {propName}.ToLower() == @{index})"
                        };
                    }
                    else
                    {
                        clause = filter.Operator.ToLower() switch
                        {
                            "eq" => $"{propName} == @{index}",
                            "gt" => $"{propName} > @{index}",
                            "lt" => $"{propName} < @{index}",
                            "gte" => $"{propName} >= @{index}",
                            "lte" => $"{propName} <= @{index}",
                            "in" when val is IEnumerable<object> => $"@{index}.Contains({propName})",
                            _ => $"{propName} == @{index}"
                        };
                    }

                    filterParts.Add(clause);
                    values.Add(val);
                    index++;
                }

                if (filterParts.Any())
                {
                    var whereClause = string.Join(" && ", filterParts);
                    queryable = queryable.Where(whereClause, values.ToArray());
                }
            }

            if (!string.IsNullOrEmpty(plan.OrderBy))
            {
                var orderProperty = queryable.ElementType.GetProperty(plan.OrderBy,
                                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (orderProperty != null)
                {
                    var parameter = Expression.Parameter(queryable.ElementType, "x");
                    var propertyExpr = Expression.Property(parameter, orderProperty);
                    var lambda = Expression.Lambda(propertyExpr, parameter);

                    bool desc = plan.OrderDirection?.ToLower() == "desc";

                    queryable = desc
                        ? Queryable.OrderByDescending((dynamic)queryable, (dynamic)lambda)
                        : Queryable.OrderBy((dynamic)queryable, (dynamic)lambda);
                }
                else
                {
                    Console.WriteLine(
                        $"Warning: OrderBy field '{plan.OrderBy}' does not exist on entity '{mappedEntity}'. Skipping ordering."
                    );
                }
            }

            if (plan.Limit.HasValue)
                queryable = queryable.Take(plan.Limit.Value);

            if (plan.Select != null && plan.Select.Any())
            {
                var fields = string.Join(",", plan.Select);
                queryable = queryable.Select($"new({fields})");
            }

            return await queryable.ToDynamicListAsync();
        }
    }
}