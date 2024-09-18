using RuleEngineEval;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RulesEngineDemoContext>();
builder.Services.AddSingleton<MicrosoftRuleEngineMatcher>();
builder.Services.AddSingleton<DiscountDemo>();
builder.Services.AddSingleton<DiscountDemoDynamic>();
builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();