using Microsoft.AspNetCore.Mvc.Testing;

namespace integrationTests;

public class ApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>()
            {
                ["WizardsApiAddress"] = "http://localhost:61656"
            });
        });
        return base.CreateHost(builder);
    }
}