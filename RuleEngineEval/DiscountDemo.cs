using RulesEngine.Models;
using System.Dynamic;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace RuleEngineEval;

public class BasicInfo
{
    public string name { get; set; }
    public string email { get; set; }
    public string creditHistory { get; set; }
    public string country { get; set; }
    public int loyaltyFactor { get; set; }
    public int totalPurchasesToDate { get; set; }
}

public class OrderInfo
{
    public int totalOrders {  get; set; }
    public int recurringItems { get; set; }
}

public class TelemetryInfo
{
    public int noOfVisitsPerMonth { get; set; }
    public int percentageOfBuyingToVisit { get; set; }
}

public class DiscountDemo
{
    public void Match()
    {
        try
        {
            Console.WriteLine($"Running {nameof(DiscountDemo)}....");
            string basicInfo = """{"name": "hello","email": "abcy@xyz.com","creditHistory": "good","country": "canada","loyaltyFactor": 1,"totalPurchasesToDate": 10000}""";
            var orderInfo = """{"totalOrders": 5,"recurringItems": 2}""";
            var telemetryInfo = """{"noOfVisitsPerMonth": 10,"percentageOfBuyingToVisit": 15}""";

            BasicInfo? input1 = JsonSerializer.Deserialize<BasicInfo>(basicInfo);
            OrderInfo input2 = JsonSerializer.Deserialize<OrderInfo>(orderInfo);
            TelemetryInfo input3 = JsonSerializer.Deserialize<TelemetryInfo>(telemetryInfo);

            var inputs = new object[]
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
            

            RuleParameter[] rParams = inputs
                    .Select((inp, i) => RuleParameter.Create<object>("input" + (i + 1), inp))
                    .ToArray();

            List<RuleResultTree> resultList = bre.ExecuteAllRulesAsync("Discount", rParams).Result;

            //ActionRuleResult result = bre.ExecuteActionWorkflowAsync(
            //    "Discount",
            //    "GiveDiscount10",
            //    rParams
            //    ).Result;
            //if (result != null)
            //{
            //    Console.WriteLine(result.ToString());
            //}

            resultList.OnSuccess((eventName) =>
            {
                discountOffered = $"Discount offered is {eventName} % over MRP.";
            });

            resultList.OnFail(() =>
            {
                discountOffered = "The user is not eligible for any discount.";
            });

            Console.WriteLine(discountOffered);
        }catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}

