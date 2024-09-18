using System.Dynamic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RulesEngine.HelperFunctions;
using RulesEngine.Models;
using Serilog;

namespace RuleEngineEval;

public class DiscountDemoDynamic
{
    private readonly RulesEngineDemoContext db;

    private RulesEngine.RulesEngine? _engine;

    public DiscountDemoDynamic(RulesEngineDemoContext db)
    {
        this.db = db;
    }

    public Workflow[]? Workflows { get; private set; }

    public DiscountRuleResult[] Match(dynamic[] inputs)
    {
        Log.Information("Matching the rule dynamically...");

        var bre = GetRulesEngine();

        string[] names = ["BasicInfo", "OrderInfo", "TelemetryInfo"];
        var prams = names
            .Select((name, idx) => new RuleParameter(name, inputs[idx]))
            .ToArray();
        return MatchRuleWithNamedParams(bre, prams);
    }

    public bool UpdateRules(Workflow[] workflows)
    {
        Workflows = workflows;
        InitializeEngine();
        return true;
    }

    private RulesEngine.RulesEngine GetRulesEngine()
    {
        if (_engine != null) return _engine;
        InitializeEngine();
        return _engine;
    }

    private void InitializeEngine()
    {
        Log.Information("Initialize the engine");
        var wf = Workflows;
        if (wf == null)
        {
            Log.Warning("There is no workflow set, use the default workflow from database");
            wf = db.Workflows.Include(i => i.Rules).ThenInclude(i => i.Rules).ToArray();
        }

        var reSettings = new ReSettings
        {
            CustomTypes = [typeof(JsonElement), typeof(Utils)]
        };
        _engine = new RulesEngine.RulesEngine(wf, reSettings);
        Log.Information("Initialize engine successfully");
    }

    private static DiscountRuleResult[] MatchRuleWithNamedParams(RulesEngine.RulesEngine bre, RuleParameter[] prams)
    {
        var resultList = bre.ExecuteAllRulesAsync("Discount", prams).Result;
        return resultList
            .Select(r =>
            {
                return new DiscountRuleResult
                {
                    RuleName = r.Rule.RuleName,
                    Result = r.IsSuccess
                        ? $"Discount offered is {r.Rule.SuccessEvent} % over MRP."
                        : "The user is not eligible for any discount."
                };
            })
            .ToArray();
    }

    public DiscountRuleResult[] MatchDynamic(dynamic? request)
    {
        Log.Information("Build the input parameters");

        var input = JsonSerializer.Deserialize<ExpandoObject>(request);
        var rParams = ((IDictionary<string, object>)input)
            .Select(e =>
            {
                if (e.Value is JsonElement jsonEle) return new RuleParameter(e.Key, jsonEle.ToExpandoObject());

                return new RuleParameter(e.Key, e.Value);
            })
            .ToArray();

        var results = MatchRuleWithNamedParams(GetRulesEngine(), rParams);
        foreach (var item in results) Log.Information("{}: {}", item.RuleName, item.Result);
        return results;
    }

    public DiscountRuleResult[] Match(DiscountRequest? request)
    {
        Log.Information("Build default BasicInfo request");
        var basicInfo =
            """{"name": "hello","email": "abcy@xyz.com","creditHistory": "good","country": "india","loyaltyFactor": 2,"totalPurchasesToDate": 12000}""";
        var orderInfo = """{"totalOrders": 5,"recurringItems": 2}""";
        var telemetryInfo = """{"noOfVisitsPerMonth": 10,"percentageOfBuyingToVisit": 15}""";


        var input1 = JsonSerializer.Deserialize<ExpandoObject>(basicInfo);
        var input2 = JsonSerializer.Deserialize<ExpandoObject>(orderInfo);
        var input3 = JsonSerializer.Deserialize<ExpandoObject>(telemetryInfo);

        //var converter = new ExpandoObjectConverter();
        //var input1 = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(basicInfo, converter);
        //var input2 = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(orderInfo, converter);
        //var input3 = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(telemetryInfo, converter);

        //var input1 = JsonConvert.DeserializeObject<ExpandoObject>(basicInfo);
        //var input2 = JsonConvert.DeserializeObject<ExpandoObject>(orderInfo);
        //var input3 = JsonConvert.DeserializeObject<ExpandoObject>(telemetryInfo);

        var results = Match([input1, input2, input3]);
        foreach (var item in results) Log.Information("{}: {}", item.RuleName, item.Result);
        return results;
    }
}