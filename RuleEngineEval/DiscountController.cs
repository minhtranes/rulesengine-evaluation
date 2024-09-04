using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace RuleEngineEval;

[ApiController]
[Route("price")]
public class DiscountController : ControllerBase
{
    private readonly DiscountDemo _matcher;
    private readonly DiscountDemoDynamic _dynamicMatcher;

    public DiscountController(DiscountDemo matcher, DiscountDemoDynamic dynamicMatcher)
    {
        _matcher = matcher;
        _dynamicMatcher = dynamicMatcher;
    }

    [HttpPost("discount")]
    public IActionResult Match(DiscountRequest? request)
    {
        try
        {
            if (request == null)
            {
                return Ok(_dynamicMatcher.Match());
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