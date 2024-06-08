using Newtonsoft.Json;

namespace EShop_Site.Components;

public static class LogMachine
{
    const string LogFileName = @"log.json";
    
    public static async Task LogRequestToJson(string message)
    {
        if (!File.Exists(LogFileName))
        {
            File.Create(LogFileName);
        }

        string logData = $"{DateTime.Now} - {message}\n";

        string json = JsonConvert.SerializeObject(logData, Formatting.Indented);

        await File.AppendAllTextAsync(LogFileName, json);
    }
}