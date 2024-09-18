using Microsoft.AspNetCore.Mvc;
using RulesEngine.Models;

namespace RuleEngineEval;

[ApiController]
[Route("workflow")]
public class RuleEngineController : ControllerBase
{
    private readonly DiscountDemoDynamic _discountDemoDynamic;

    public RuleEngineController(DiscountDemoDynamic discountDemoDynamic)
    {
        _discountDemoDynamic = discountDemoDynamic;
    }

    [HttpGet("current")]
    public IActionResult Workflow()
    {
        return Ok(_discountDemoDynamic.Workflows);
    }

    [HttpPost("update")]
    public IActionResult Update(Workflow[] workflows)
    {
        return Ok(_discountDemoDynamic.UpdateRules(workflows));
    }
}