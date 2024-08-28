using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RulesEngine.Models;

namespace RuleEngineEval;

[ApiController]
[Route("rule")]
public class RuleEngineController : ControllerBase
{
    private RulesEngineDemoContext _context;

    public RuleEngineController(RulesEngineDemoContext context)
    {
        _context = context;
    }

    [HttpGet("workflow")]
    public IActionResult Workflow()
    {
        var wk = _context.Workflows.Include(i => i.Rules).ThenInclude(i => i.Rules).ToArray();
        return Ok(wk);
    }

    [HttpPost("workflow")]
    public IActionResult Update(Workflow[] workflows)
    {
        foreach (var item in workflows)
        {
            _context.Update(item);
        }
        _context.SaveChanges();
        return Ok();
    }
}