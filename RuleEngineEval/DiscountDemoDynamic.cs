using RulesEngine.Models;
using System.Dynamic;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Serilog;


namespace RuleEngineEval;

public class DiscountDemoDynamic
{
    private RulesEngineDemoContext db;

    public DiscountDemoDynamic(RulesEngineDemoContext db)
    {
        this.db = db;
    }

    public DiscountRuleResult[] Match(dynamic[] inputs)
    {

        Log.Information("Matching the rule dynamically...");

        var wfr = db.Workflows.Include(i => i.Rules).ThenInclude(i => i.Rules).ToArray();
        var reSettings = new ReSettings()
        {
            CustomTypes = new Type[] { typeof(System.Text.Json.JsonElement) },
            EnableScopedParams = false,
            IgnoreException=true,
            AutoRegisterInputType = true,

        };
        var bre = new RulesEngine.RulesEngine(wfr, null);

        var rParams = inputs
                .Select((inp, idx) => RuleParameter.Create<dynamic>($"input{idx+1}", inp))
                .ToArray();
        var resultList = bre.ExecuteAllRulesAsync("Discount", rParams).Result;
        
        return resultList
            .Select(r =>
            {
                return new DiscountRuleResult
                {
                    RuleName = r.Rule.RuleName,
                    Result = r.IsSuccess ? $"Discount offered is {r.Rule.SuccessEvent} % over MRP." : "The user is not eligible for any discount."
                };
            })
            .ToArray();

    }
    public DiscountRuleResult[] Match()
    {
        Log.Information("Build default BasicInfo request");
        string basicInfo = """{"name": "hello","email": "abcy@xyz.com","creditHistory": "good","country": "india","loyaltyFactor": 3,"totalPurchasesToDate": 12000}""";
        string orderInfo = """{"totalOrders": 5,"recurringItems": 2}""";
        string telemetryInfo = """{"noOfVisitsPerMonth": 10,"percentageOfBuyingToVisit": 15}""";

        var input2 = JsonSerializer.Deserialize<dynamic>(orderInfo);
        var input3 = JsonSerializer.Deserialize<dynamic>(telemetryInfo);
        var input1 = JsonSerializer.Deserialize<dynamic>(basicInfo);
        
        DiscountRuleResult[] results = Match([input1, input2, input3]);
        foreach (var item in results)
        {
            Log.Information("{}: {}", item.RuleName, item.Result);
        }
        return results;
    }
}

