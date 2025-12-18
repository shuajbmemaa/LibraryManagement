using LMS.Application.DTO.AI;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace LMS.Application.Services.AI
{
    //public class AiQueryService : IAiQueryService
    //{
    //    private readonly IBookRepository _books;
    //    private readonly IUserRepository _users;

    //    public AiQueryService(IBookRepository books, IUserRepository users)
    //    {
    //        _books = books;
    //        _users = users;
    //    }

        //public async Task<object> QueryAsync(string question)
        //{
        //    var plan = ParseQueryPlan(question);
        //    return await ExecuteQueryPlanAsync(plan);
        //}

        //private QueryPlan ParseQueryPlan(string question)
        //{
        //    question = question.ToLower();
        //    var plan = new QueryPlan
        //    {
        //        Filter = new Dictionary<string, object>(),
        //        Select = new List<string>()
        //    };

        //    if (question.Contains("book") || question.Contains("books")) plan.Entity = "Books";
        //    else if (question.Contains("user") || question.Contains("owner")) plan.Entity = "Users";
        //    else throw new Exception("Cannot determine entity");

        //    Type entityType = plan.Entity == "Books" ? typeof(Domain.Entities.Book) : typeof(ApplicationUser);
        //    var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    var keywordToProperty = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        //    {
        //        { "title", "Title" },
        //        { "author", "Author" },
        //        { "reading status", "ReadingStatus" },
        //        { "genre", "Genre" },
        //        { "created by", "UserId" },
        //        { "user id", "UserId" },
        //        { "books count", "Books.Count" }
        //    };

        //    foreach (var kvp in keywordToProperty)
        //    {
        //        if (question.Contains(kvp.Key))
        //        {
        //            var idx = question.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) + kvp.Key.Length;
        //            var valueStr = question.Substring(idx).Trim();

        //            var separators = new string[] { " with ", " by ", " created by ", " which " };
        //            foreach (var sep in separators)
        //            {
        //                if (valueStr.Contains(sep))
        //                    valueStr = valueStr.Substring(0, valueStr.IndexOf(sep)).Trim();
        //            }

        //            if (kvp.Value == "ReadingStatus")
        //            {
        //                var normalized = valueStr.Replace(" ", "");
        //                if (Enum.TryParse(typeof(ReadingStatus), normalized, true, out var enumVal))
        //                    plan.Filter.Add(kvp.Value, enumVal);
        //            }
        //            else
        //            {
        //                plan.Filter.Add(kvp.Value, valueStr);
        //            }
        //        }
        //    }

        //    if (question.Contains("top"))
        //    {
        //        plan.Limit = ExtractTopN(question);
        //        if (!string.IsNullOrEmpty(plan.Entity))
        //            plan.OrderBy = properties.FirstOrDefault()?.Name;
        //        plan.OrderDirection = "asc";
        //    }

        //    if (!plan.Select.Any())
        //        plan.Select.AddRange(properties.Select(p => p.Name));

        //    return plan;
        //}

        //private int ExtractTopN(string question, int defaultN = 5)
        //{
        //    var words = question.Split(' ');
        //    for (int i = 0; i < words.Length; i++)
        //    {
        //        if (words[i] == "top" && i + 1 < words.Length && int.TryParse(words[i + 1], out int n))
        //            return n;
        //    }
        //    return defaultN;
        //}

        //private async Task<object> ExecuteQueryPlanAsync(QueryPlan plan)
        //{
        //    IQueryable<dynamic> query = plan.Entity switch
        //    {
        //        "Books" => _books.GetAll(),
        //        "Users" => _users.GetAll(),
        //        _ => throw new Exception("Unknown entity")
        //    };

        //    if (plan.Filter != null)
        //    {
        //        foreach (var f in plan.Filter)
        //        {
        //            if (f.Key == "UserId")
        //            {
        //                query = query.Where($"{f.Key} == @0", Guid.Parse(f.Value.ToString()));
        //            }
        //            else if (f.Value is string s)
        //            {
        //                query = query.Where($"{f.Key}.ToLower() == @0", s.ToLower());
        //            }
        //            else
        //            {
        //                query = query.Where($"{f.Key} == @0", f.Value);
        //            }
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(plan.OrderBy))
        //    {
        //        var direction = plan.OrderDirection?.ToLower() == "desc" ? "descending" : "ascending";
        //        query = query.OrderBy($"{plan.OrderBy} {direction}");
        //    }

        //    if (plan.Limit != null)
        //        query = query.Take(plan.Limit.Value);

        //    var list = await query.ToListAsync();

        //    if (plan.Select != null && plan.Select.Any())
        //    {
        //        var projected = list.Select(item =>
        //        {
        //            var dict = new Dictionary<string, object?>();
        //            foreach (var field in plan.Select)
        //            {
        //                var prop = item.GetType().GetProperty(field);
        //                if (prop != null)
        //                    dict[field] = prop.GetValue(item);
        //            }
        //            return dict;
        //        }).ToList();

        //        return projected;
        //    }

        //    return list;
        //}
//    }
}