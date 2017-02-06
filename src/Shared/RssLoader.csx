#r "System.Xml.Linq"
#r "Newtonsoft.Json"
using System.Linq;
using System.Xml.Linq;

public static bool TryLoadRssFeed(out string result)
{
    try
    {
        var xmlDoc = XDocument.Load("https://medium.com/feed/@devlead");
        var feed = (
                        from channel in xmlDoc.Root?.Elements("channel")
                        select new {
                                title = channel.Elements("title").Select(node => node.Value).FirstOrDefault(),
                                items = (
                                            from item in channel.Elements("item")
                                            where item.Elements("category").Any()
                                            select new {
                                                title = item.Elements("title").Select(node => node.Value).FirstOrDefault(),
                                                link = item.Elements("link").Select(node => node.Value).FirstOrDefault(),
                                                pubDate = item.Elements("pubDate").Select(
                                                    node => {
                                                        DateTimeOffset parsedDate;
                                                        return (DateTimeOffset.TryParse(node.Value, out parsedDate))
                                                                ? parsedDate.ToString("yyyy-MM-dd")
                                                                : node.Value;
                                                        
                                                }).FirstOrDefault()
                                            }
                                        ).Take(10)
                                        .ToArray()
                        }
            ).FirstOrDefault();

        result = Newtonsoft.Json.JsonConvert.SerializeObject(feed);
        return true;
    }
    catch(Exception ex)
    {
        result = ex.ToString();
        return false;
    }
}