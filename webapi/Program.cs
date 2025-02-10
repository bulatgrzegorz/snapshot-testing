using exampleProject;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration.GetValue<bool>("IsSnapshotMode"))
{
    builder.Services.TryAddTransient<RecorderDelegatingHandler>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.TryAddScoped<Recorder>();
    builder.Services.ConfigureAll<HttpClientFactoryOptions>(o =>
        o.HttpMessageHandlerBuilderActions.Add(b => b.AdditionalHandlers.Insert(b.AdditionalHandlers.Count, b.Services.GetRequiredService<RecorderDelegatingHandler>())));
}

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/spellsAndIngredients", async (IConfiguration configuration, IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient();
    httpClient.BaseAddress = new Uri(configuration.GetValue<string>("WizardsApiAddress")!);

    var spells = httpClient.GetFromJsonAsync<Spell[]>("Spells?Type=Spell");
    var ingredients = httpClient.GetFromJsonAsync<Ingredient[]>("Ingredients?Name=Dragon");
    
    await Task.WhenAll(spells, ingredients);
    
    return new {Spells = spells.Result, Ingredients = ingredients.Result};
})
.WithName("SpellsAndIngredients");

app.Run();

public partial class Program { }
internal record Spell(Guid Id, string Name, string Effect);
internal record Ingredient(Guid Id, string Name);