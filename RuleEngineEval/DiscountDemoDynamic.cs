using Microsoft.EntityFrameworkCore;
using RulesEngine.Models;
using Serilog;
using System.Collections;
using System.Dynamic;
using System.Linq.Dynamic.Core;

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
            CustomTypes = [typeof(System.Text.Json.JsonElement), typeof(Utils)]
        };
        var bre = new RulesEngine.RulesEngine(wfr, reSettings);

        //string[] names = ["BasicInfo", "OrderInfo", "TelemetryInfo"];
        //var prams = names
        //        .Select((name, idx) => RuleParameter.Create<dynamic>(name, inputs[idx]))
        //        .ToArray();
        //var resultList = bre.ExecuteAllRulesAsync("Discount", prams).Result;

        //var rParams = inputs
        //        .Select((inp, idx) => RuleParameter.Create<dynamic>($"input{idx + 1}", inp))
        //        .ToArray();
        var resultList = bre.ExecuteAllRulesAsync("Discount", inputs).Result;


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

    private static Type AbcCreateAbstractClassType(dynamic input)
    {
        List<DynamicProperty> props = new List<DynamicProperty>();

        if (input == null)
        {
            return typeof(object);
        }
        if (!(input is ExpandoObject))
        {
            return input.GetType();
        }

        else
        {
            foreach (var expando in (IDictionary<string, object>)input)
            {
                Type value;
                if (expando.Value is IList)
                {
                    if (((IList)expando.Value).Count == 0)
                        value = typeof(List<object>);
                    else
                        value = typeof(List<object>);
                    //else
                    //{
                    //    var internalType = AbcCreateAbstractClassType(((IList)expando.Value)[0]);
                    //    value = new List<object>().Cast(internalType).ToList(internalType).GetType();
                    //} 

                }
                else
                {
                    value = AbcCreateAbstractClassType(expando.Value);
                }
                props.Add(new DynamicProperty(expando.Key, value));
            }
        }

        var type = DynamicClassFactory.CreateType(props);
        return type;
    }

    public DiscountRuleResult[] Match()
    {
        Log.Information("Build default BasicInfo request");
        string basicInfo = """{"name": "hello","email": "abcy@xyz.com","creditHistory": "good","country": "india","loyaltyFactor": 2,"totalPurchasesToDate": 12000}""";
        string orderInfo = """{"totalOrders": 5,"recurringItems": 2}""";
        string telemetryInfo = """{"noOfVisitsPerMonth": 10,"percentageOfBuyingToVisit": 15}""";


        var input1 = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(basicInfo);
        var input2 = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(orderInfo);
        var input3 = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(telemetryInfo);

        //var converter = new ExpandoObjectConverter();
        //var input1 = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(basicInfo, converter);
        //var input2 = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(orderInfo, converter);
        //var input3 = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(telemetryInfo, converter);

        //var input1 = JsonConvert.DeserializeObject<ExpandoObject>(basicInfo);
        //var input2 = JsonConvert.DeserializeObject<ExpandoObject>(orderInfo);
        //var input3 = JsonConvert.DeserializeObject<ExpandoObject>(telemetryInfo);

        DiscountRuleResult[] results = Match([input1, input2, input3]);
        foreach (var item in results)
        {
            Log.Information("{}: {}", item.RuleName, item.Result);
        }
        return results;
    }
}

