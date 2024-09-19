using Microsoft.AspNetCore.Mvc;
using RuleEngineEval.Service;
using RulesEngine.Models;

namespace RuleEngineEval;

[ApiController]
[Route("workflow")]
public class RuleEngineController : ControllerBase
{
    private readonly IRuleEngineService RuleEngineService;

    public RuleEngineController(MicrosoftRulesEngineExService rulesEngineService)
    {
        RuleEngineService = rulesEngineService;
    }

    [HttpGet("current")]
    public IActionResult Workflow()
    {
        return Ok(RuleEngineService.CurrentWorkflows());
    }

    [HttpPost("update")]
    public IActionResult Update(Workflow[] workflows)
    {
        return Ok(RuleEngineService.UpdateRules(workflows));
    }
}