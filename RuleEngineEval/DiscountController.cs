using Microsoft.AspNetCore.Mvc;

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
    public void Match()
    {
        _matcher.Match();
    }
}