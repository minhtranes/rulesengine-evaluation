using Microsoft.AspNetCore.Mvc;
using RuleEngineEval.Service;
using Serilog;

namespace RuleEngineEval.Controller;

[ApiController]
[Route("rules")]
public class RulesEngineController : ControllerBase
{
    private readonly IRulesEngineService _rulesEngineService;

    public RulesEngineController(IRulesEngineService rulesEngineService)
    {
        _rulesEngineService = rulesEngineService;
    }

    [HttpPost("execute")]
    public IActionResult Match([FromBody] dynamic? request)
    {
        try
        {
            return Ok(_rulesEngineService.Match(request));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to execute the rule");
            return StatusCode(500, "Failed to execute the rule matching");
        }
    }
}