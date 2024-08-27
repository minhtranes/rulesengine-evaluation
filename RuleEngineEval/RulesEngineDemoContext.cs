using Microsoft.EntityFrameworkCore;

namespace RuleEngineEval;

public class RulesEngineDemoContext: RulesEngineContext
{
    public string DbPath { get; private set; }

    public RulesEngineDemoContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}RulesEngineDemo.db";
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
