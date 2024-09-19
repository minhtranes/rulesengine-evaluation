using Microsoft.AspNetCore.Mvc;
using RuleEngineEval.Service;
using Serilog;

namespace RuleEngineEval;

[ApiController]
[Route("rules")]
public class RulesEngineController : ControllerBase
{
    private readonly MicrosoftRulesEngineExService _dynamicMatcher;

    public RulesEngineController(MicrosoftRulesEngineExService dynamicMatcher)
    {
        _dynamicMatcher = dynamicMatcher;
    }

    [HttpPost("execute")]
    public IActionResult Match([FromBody] dynamic? request)
    {
        try
        {
            return Ok(_dynamicMatcher.Match(request));
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            return StatusCode(500, "Failed to execute the rule matching");
        }
    }
}