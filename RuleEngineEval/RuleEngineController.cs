using Microsoft.AspNetCore.Mvc;

namespace RuleEngineEval;

[ApiController]
[Route("[controller]")]
public class RuleEngineController : ControllerBase
{
    private readonly MicrosoftRuleEngineMatcher _matcher;

    private readonly string[] summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public RuleEngineController(MicrosoftRuleEngineMatcher matcher)
    {
        _matcher = matcher;
    }

    [HttpPost("Match")]
    public void Match()
    {
        _matcher.Match();
    }

    [HttpGet("Forecast")]
    public WeatherForecast Forecast()
    {
        return Enumerable
            .Range(1, 5)
            .Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .First();
    }

    [HttpGet("DoForecast")]
    public WeatherForecast DoForecast()
    {
        return Enumerable
            .Range(1, 5)
            .Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .First();
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}