using System.Collections.Specialized;
using System.Web;

namespace integrationTests;

public static class UriHelper
{
    public static bool ParseRelativeUri(string relativeUri, out Uri? result, out NameValueCollection? queryParams)
    {
        //relative uri are not being parse into parts, we are creating du
        var parsed = Uri.TryCreate($"http://localhost{relativeUri}", UriKind.Absolute, out result);
        if (!parsed)
        {
            result = null;
            queryParams = null;
            return false;
        }
        
        queryParams = HttpUtility.ParseQueryString(result!.Query);
        return true;
    }
}