using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace RuleEngineEval;

[ApiController]
[Route("price")]
public class DiscountController : ControllerBase
{
    private readonly DiscountDemo _matcher;

    public DiscountController(DiscountDemo matcher)
    {
        _matcher = matcher;
    }

    [HttpPost("discount")]
    public IActionResult Match(DicountRequest? request)
    {
        try
        {
            if (request == null)
            {
                return Ok(_matcher.Match());
            }
            else
            {
                return Ok(_matcher.Match(request));
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            return StatusCode(500, "Failed to execute the rule matching");
        }
    }
}