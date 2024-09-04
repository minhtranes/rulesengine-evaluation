using RulesEngine.Models;
using System.Dynamic;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Serilog;


namespace RuleEngineEval;

public class DiscountRequest
{
    public BasicInfo BasicInfo { get; set; }
    public OrderInfo OrderInfo { get; set; }
    public TelemetryInfo TelemetryInfo { get; set; }
}

public class DiscountRuleResult
{
    public string RuleName { get; set; }
    public string Result { get; set; }
}

public class BasicInfo
{
    public string name { get; set; }
    public string email { get; set; }
    public string creditHistory { get; set; }
    public string country { get; set; }
    public int loyaltyFactor { get; set; }
    public int totalPurchasesToDate { get; set; }
}

public class OrderInfo
{
    public int totalOrders { get; set; }
    public int recurringItems { get; set; }
}

public class TelemetryInfo
{
    public int noOfVisitsPerMonth { get; set; }
    public int percentageOfBuyingToVisit { get; set; }
}

public static class Utils
{
    public static bool isValidEmail(string email)
    {
        return email.Contains("@");
    }
}

public class DiscountDemo
{
    private RulesEngineDemoContext db;

    public DiscountDemo(RulesEngineDemoContext db)
    {
        this.db = db;
    }

    public DiscountRuleResult[] Match(DiscountRequest dicountRequest)
    {

        Log.Information("Matching the rule...");

        var inputs = new object[]
            {
                        dicountRequest.BasicInfo,
                        dicountRequest.OrderInfo,
                        dicountRequest.TelemetryInfo
            };

        var wfr = db.Workflows.Include(i => i.Rules).ThenInclude(i => i.Rules).ToArray();
        var reSettings = new ReSettings()
        {
            CustomTypes = new Type[]{typeof(Utils)}
        };
        var bre = new RulesEngine.RulesEngine(wfr, reSettings);

        RuleParameter[] rParams = inputs
                .Select((inp, i) => RuleParameter.Create<object>("input" + (i + 1), inp))
                .ToArray();

        List<RuleResultTree> resultList = bre.ExecuteAllRulesAsync("Discount", rParams).Result;
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
        string basicInfo = """{"name": "hello","email": "abcy@xyz.com","creditHistory": "good","country": "canada","loyaltyFactor": 1,"totalPurchasesToDate": 10000}""";
        string orderInfo = """{"totalOrders": 5,"recurringItems": 2}""";
        string telemetryInfo = """{"noOfVisitsPerMonth": 10,"percentageOfBuyingToVisit": 15}""";

        OrderInfo input2 = JsonSerializer.Deserialize<OrderInfo>(orderInfo);
        TelemetryInfo input3 = JsonSerializer.Deserialize<TelemetryInfo>(telemetryInfo);
        BasicInfo input1 = JsonSerializer.Deserialize<BasicInfo>(basicInfo);

        DiscountRuleResult[] results = Match(new DiscountRequest { BasicInfo = input1, OrderInfo = input2, TelemetryInfo = input3 });
        foreach (var item in results)
        {
            Log.Information("{}: {}", item.RuleName, item.Result);
        }
        return results;
    }
}

