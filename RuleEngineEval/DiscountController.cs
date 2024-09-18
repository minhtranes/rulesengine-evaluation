using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace RuleEngineEval;

public class RuleValidationRequest
{
    private dynamic input { get; set; }
}

[ApiController]
[Route("price")]
public class DiscountController : ControllerBase
{
    private readonly DiscountDemoDynamic _dynamicMatcher;
    private readonly DiscountDemo _matcher;

    public DiscountController(DiscountDemo matcher, DiscountDemoDynamic dynamicMatcher)
    {
        _matcher = matcher;
        _dynamicMatcher = dynamicMatcher;
    }

    [HttpPost("discount")]
    public IActionResult Match([FromBody] dynamic? request)
    {
        try
        {
            // if (request == null)
            // {
            //     return Ok(_dynamicMatcher.Match(request));
            // }
            // else
            // {
            //     return Ok(_matcher.Match(request));
            // }
            return Ok(_dynamicMatcher.MatchDynamic(request));
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            return StatusCode(500, "Failed to execute the rule matching");
        }
    }
}