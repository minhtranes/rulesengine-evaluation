using Microsoft.EntityFrameworkCore;
using RulesEngine.Models;
using System.Text.Json;

namespace RuleEngineEval;

public class RulesEngineDemoContext : RulesEngineContext
{
    public string DbPath { get; private set; }

    public RulesEngineDemoContext()
    {
        var path = Directory.GetCurrentDirectory() + "/Data/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}RulesEngineDemo.db";

        var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "Discount1.json", SearchOption.AllDirectories);
        if (files == null || files.Length == 0)
            throw new Exception("Rules not found.");
        var fileData = File.ReadAllText(files[0]);
        var workflow = JsonSerializer.Deserialize<List<Workflow>>(fileData);
        if (Database.EnsureCreated())
        {
            Workflows.AddRange(workflow);
            SaveChanges();
        }
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
