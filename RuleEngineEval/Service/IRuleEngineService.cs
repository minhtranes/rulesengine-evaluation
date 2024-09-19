using RulesEngine.Models;

namespace RuleEngineEval.Service;

public interface IRuleEngineService
{
    public DiscountRuleResult[] Match(dynamic? request);

    Workflow[] CurrentWorkflows();
    object? UpdateRules(Workflow[] workflows);
}
