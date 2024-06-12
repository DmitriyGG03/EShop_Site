using System.Data;
using Newtonsoft.Json;

namespace EShop_Site.Helpers;

public static class JsonHelper
{
    public static async Task<T> GetTypeFromResponseAsync<T>(HttpResponseMessage res)
    {
        var jsonString = await res.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(jsonString) ??
                   throw new DataException("Json deserialize error!");
    }
}