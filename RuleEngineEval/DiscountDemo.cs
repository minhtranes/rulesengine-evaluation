﻿using RulesEngine.Models;
using System.Dynamic;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace RuleEngineEval;

public class DiscountDemo
{
    public void Match()
    {
        Console.WriteLine($"Running {nameof(DiscountDemo)}....");
        var basicInfo = "{\"name\": \"hello\",\"email\": \"abcy@xyz.com\",\"creditHistory\": \"good\",\"country\": \"canada\",\"loyaltyFactor\": 3,\"totalPurchasesToDate\": 10000}";
        var orderInfo = "{\"totalOrders\": 5,\"recurringItems\": 2}";
        var telemetryInfo = "{\"noOfVisitsPerMonth\": 10,\"percentageOfBuyingToVisit\": 15}";

        dynamic input1 = JsonSerializer.Deserialize<ExpandoObject>(basicInfo);
        dynamic input2 = JsonSerializer.Deserialize<ExpandoObject>(orderInfo);
        dynamic input3 = JsonSerializer.Deserialize<ExpandoObject>(telemetryInfo);

        var inputs = new dynamic[]
            {
                    input1,
                    input2,
                    input3
            };

        var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "Discount.json", SearchOption.AllDirectories);
        if (files == null || files.Length == 0)
            throw new Exception("Rules not found.");

        var fileData = File.ReadAllText(files[0]);
        var workflow = JsonSerializer.Deserialize<List<Workflow>>(fileData);

        RulesEngineDemoContext db = new RulesEngineDemoContext();
        if (db.Database.EnsureCreated())
        {
            db.Workflows.AddRange(workflow);
            db.SaveChanges();
        }

        var wfr = db.Workflows.Include(i => i.Rules).ThenInclude(i => i.Rules).ToArray();

        var bre = new RulesEngine.RulesEngine(wfr, null);
        string discountOffered = "No discount offered.";

        List<RuleResultTree> resultList = bre.ExecuteAllRulesAsync("Discount", inputs).Result;

        resultList.OnSuccess((eventName) => {
            discountOffered = $"Discount offered is {eventName} % over MRP.";
        });

        resultList.OnFail(() => {
            discountOffered = "The user is not eligible for any discount.";
        });

        Console.WriteLine(discountOffered);
    }
}

