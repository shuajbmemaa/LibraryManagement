using LMS.Application.DTO.AI;

namespace LMS.Application.Services.AI
{
    public interface IOllamaQueryService
    {
        Task<QueryPlan?> GenerateQueryPlanAsync(string question);
        Task<(List<object> Data, int Count)> ExecuteQueryPlanAsync(QueryPlan plan);
    }
}