using LMS.Application.DTO.AI;
using LMS.Application.Services.AI;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiQueryController : ControllerBase
    {
        private readonly IOllamaQueryService _oaQueryService;

        public AiQueryController(IOllamaQueryService oaQueryService)
        {
            _oaQueryService = oaQueryService;
        }

        [HttpPost("ollama-query")]
        public async Task<IActionResult> OllamaQuery(AiQueryRequest req)
        {
            var plan = await _oaQueryService.GenerateQueryPlanAsync(req.Question);
            if (plan == null)
                return BadRequest("Su bo format mire.");

            var result = await _oaQueryService.ExecuteQueryPlanAsync(plan);

            var list = result as IEnumerable<object>;
            
            var count = list?.Count() ?? 0;

            if (count == 0)
            {
                return Ok(new
                {
                    answer = "No results found.",
                    raw = result
                });
            }

            var humanAnswer = await _oaQueryService.FormatResultAsync(
                req.Question,
                plan.Entity,
                result,
                count
            );

            return Ok(new{
                answer = humanAnswer,
                raw = result});
        }
    }
}
