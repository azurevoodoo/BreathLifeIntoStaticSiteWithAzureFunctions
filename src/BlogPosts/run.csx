#load "../Shared/RssLoader.csx"
using System.Net;
using System.Net.Http.Headers;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    var feed = await Task.Run(()=>{
        string result = null;
        if (!TryLoadRssFeed(out result))
        {
            log.Error(result);
            result = "{}";
        }
        return $"var feed = {result}"; 
    });

    var res = req.CreateResponse(
        HttpStatusCode.OK
    );

    res.Content = new StringContent(
        feed,
        System.Text.Encoding.UTF8,
        "text/javascript"
        );

    res.Headers.CacheControl = FeedCacheControl;

    return res;
}

private static readonly CacheControlHeaderValue FeedCacheControl = new CacheControlHeaderValue {
        MaxAge = new TimeSpan(0, 10, 0),
        Public = true
    };