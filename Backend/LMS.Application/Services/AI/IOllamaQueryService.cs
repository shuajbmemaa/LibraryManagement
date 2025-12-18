using LMS.Application.DTO.AI;

namespace LMS.Application.Services.AI
{
    public interface IOllamaQueryService
    {
        Task<QueryPlan?> GenerateQueryPlanAsync(string question);
        Task<object> ExecuteQueryPlanAsync(QueryPlan plan);
        Task<string> FormatResultAsync(string question, string entity, object data, int count);
    }
}