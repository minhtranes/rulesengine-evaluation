using Microsoft.EntityFrameworkCore;
using RulesEngine.HelperFunctions;
using RulesEngine.Models;
using Serilog;
using System.Dynamic;
using System.Text.Json;

namespace RuleEngineEval.Service;

public class DiscountRuleResult
{
    public string RuleName { get; set; }
    public string Result { get; set; }
}

public class MicrosoftRulesEngineExService : IRulesEngineService
{
    private readonly RulesEngineDemoContext _context;

    private RulesEngine.RulesEngine _engine;

    public MicrosoftRulesEngineExService(RulesEngineDemoContext db)
    {
        this._context = db;
        InitializeEngine();
    }

    private Workflow[] _workflows;

    public bool UpdateRules(Workflow[] workflows)
    {
        _workflows = workflows;
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
        var wf = _workflows;
        if (wf == null)
        {
            Log.Warning("There is no workflow set, use the default workflow from database");
            wf = _context.Workflows.Include(i => i.Rules).ThenInclude(i => i.Rules).ToArray();
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

    public DiscountRuleResult[] Match(dynamic? request)
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

    public Workflow[] CurrentWorkflows()
    {
        return _workflows;
    }

}