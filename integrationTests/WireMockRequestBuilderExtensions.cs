using WireMock.RequestBuilders;

namespace integrationTests;

public static class WireMockRequestBuilderExtensions
{
    public static IRequestBuilder WithMethod(this IRequestBuilder requestBuilder, string method)
    {
        return method switch
        {
            "GET" => requestBuilder.UsingGet(),
            "POST" => requestBuilder.UsingPost(),
            "PUT" => requestBuilder.UsingPut(),
            "DELETE" => requestBuilder.UsingDelete(),
            _ => throw new ArgumentException($"Method {method} is not supported")
        };
    }

    public static IRequestBuilder WithRequestBody(this IRequestBuilder requestBuilder, string? body)
    {
        return string.IsNullOrEmpty(body) ? requestBuilder : requestBuilder.WithBody(body);
    }

    public static IRequestBuilder WithRelativePath(this IRequestBuilder requestBuilder, string path)
    {
        var parsed = UriHelper.ParseRelativeUri(path, out var uri, out var queryParams);
        if(!parsed) throw new ArgumentException($"Path {path} is not valid");
        
        requestBuilder = requestBuilder.WithPath(uri!.LocalPath);
        foreach (string queryParameter in queryParams ?? [])
        {
            requestBuilder = requestBuilder.WithParam(queryParameter, queryParams![queryParameter]!);
        }

        return requestBuilder;
    }
}