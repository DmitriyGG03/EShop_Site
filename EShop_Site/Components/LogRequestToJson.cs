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

        using (StreamWriter file = new StreamWriter(LogFileName, true))
        {
            string logData = $"{DateTime.Now} - {message}";

            string json = JsonConvert.SerializeObject(logData);
            
            await file.WriteLineAsync(json);
        }
    }
}