using System.Collections.Concurrent;
using System.Text.Json;

namespace exampleProject;

public class RecorderDelegatingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        var recorder = httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<Recorder>();
        if (recorder is null) return response;
        
        var address = Uri.UnescapeDataString(request.RequestUri!.PathAndQuery);
        var method = request.Method.ToString();
        var responseStatusCode = (int)response.StatusCode;
        var requestContent = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        
        recorder.AddSnapshot(new Snapshot(address, method, requestContent, responseContent, responseStatusCode));
        
        return response;
    }
}
public record Snapshot(string Address, string Method, string? RequestBody, string ResponseBody, int StatusCode);
public class Recorder : IAsyncDisposable
{
    private readonly ConcurrentBag<Snapshot> _snapshots = [];
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
    public void AddSnapshot(Snapshot snapshot) => _snapshots.Add(snapshot);

    public async ValueTask DisposeAsync()
    {
        var serializedSnapshot = JsonSerializer.Serialize(_snapshots.OrderBy(x => x.Address), JsonSerializerOptions);

        await File.WriteAllTextAsync("snapshot.json", serializedSnapshot);
    }
}