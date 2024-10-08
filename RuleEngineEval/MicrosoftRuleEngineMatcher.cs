using RulesEngine.Actions;
using RulesEngine.Models;
using Serilog;

namespace RuleEngineEval;

public class MicrosoftRuleEngineMatcher
{
    private readonly RulesEngine.RulesEngine _rulesEngine;

    public MicrosoftRuleEngineMatcher()
    {
        var reSettings = new ReSettings
        {
            CustomActions = new Dictionary<string, Func<ActionBase>> { { "LogWarningAction", () => new LogWarningAction() } }
        };
        var workflowRules = File.ReadAllText("cpu_alert_rule.json");
        _rulesEngine = new RulesEngine.RulesEngine(new[] { workflowRules }, reSettings);
        Log.Information("Initialized the rule engine");
    }

    public async void Match()
    {
        // Create an instance of the input class
        var vm = new VirtualMachine { CPUUsage = 85 }; // Example CPU usage

        // Execute the workflow rules with the input
        var resultList = await _rulesEngine.ExecuteAllRulesAsync("CPUAlertWorkflow", new VirtualMachine { CPUUsage = 85 });

        // Check the results
        foreach (var result in resultList)
        {
            Log.Information("Result: "+result.IsSuccess);
            Log.Information("Action R: " + result.ActionResult.Exception);
        }
            
    }
}