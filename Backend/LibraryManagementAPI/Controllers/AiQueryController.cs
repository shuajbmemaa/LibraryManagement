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

            var (data, count) = await _oaQueryService.ExecuteQueryPlanAsync(plan);

            var message = ChatResponseBuilder.Build(
                plan.Chat,
                data,
                count
            );
            return Ok(new{message, data});
        }
    }
}
