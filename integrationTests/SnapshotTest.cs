using System.Text.Json;
using exampleProject;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace integrationTests;

public class SnapshotTest(ApplicationFactory<Program> factory) : IClassFixture<ApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });

    [Fact]
    public async Task Should_Get_Snapshot_Spells()
    {
        using var wireMock = WireMockServer.Start(port: 61656, useSSL: false);
        
        var path = "snapshot.json";
        var snapshots =  JsonSerializer.DeserializeAsyncEnumerable<Snapshot>(File.OpenRead(path));

        await foreach (var snapshot in snapshots)
        {
            wireMock
                .Given(Request.Create()
                    .WithRelativePath(snapshot!.Address)
                    .WithRequestBody(snapshot.RequestBody)
                    .WithMethod(snapshot.Method))
                .RespondWith(Response.Create()
                    .WithStatusCode(snapshot.StatusCode)
                    .WithBody(snapshot.ResponseBody));
        }
        
        var httpResponse = await _client.GetAsync("spellsAndIngredients");
        var contentResponse = await httpResponse.Content.ReadAsStringAsync();
        
        await Verify(contentResponse.ToIndentedJson());
    }
}