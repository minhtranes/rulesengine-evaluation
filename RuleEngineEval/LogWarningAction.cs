using RulesEngine.Actions;
using RulesEngine.Models;
using Serilog;

namespace RuleEngineEval;

public class LogWarningAction : ActionBase
{
    public override ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters)
    {
        var message = context.GetContext<string>("message");
        Log.Warning($"WARN {message}");
        Log.Information("Running...");
        return new ValueTask<object>(ValueTask.CompletedTask);
    }
}