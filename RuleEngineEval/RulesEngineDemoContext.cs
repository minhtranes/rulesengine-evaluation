using Microsoft.EntityFrameworkCore;

namespace RuleEngineEval;

public class RulesEngineDemoContext: RulesEngineContext
{
    public string DbPath { get; private set; }

    public RulesEngineDemoContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Directory.GetCurrentDirectory()+"/Data/";// Environment.GetFolderPath(folder);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}RulesEngineDemo.db";
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
