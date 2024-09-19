using Microsoft.AspNetCore.Mvc;
using RuleEngineEval.Service;
using RulesEngine.Models;

namespace RuleEngineEval.Controller;

[ApiController]
[Route("workflow")]
public class WorkflowController : ControllerBase
{
    private readonly IRulesEngineService _rulesEngineService;

    public WorkflowController(IRulesEngineService rulesEngineService)
    {
        _rulesEngineService = rulesEngineService;
    }

    [HttpGet("current")]
    public IActionResult Workflow()
    {
        return Ok(_rulesEngineService.CurrentWorkflows());
    }

    [HttpPost("update")]
    public IActionResult Update(Workflow[] workflows)
    {
        return Ok(_rulesEngineService.UpdateRules(workflows));
    }
}