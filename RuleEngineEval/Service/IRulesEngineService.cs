using RulesEngine.Models;

namespace RuleEngineEval.Service;

public interface IRulesEngineService
{
    DiscountRuleResult[] Match(dynamic? request);
    Workflow[] CurrentWorkflows();
    bool UpdateRules(Workflow[] workflows);
}
