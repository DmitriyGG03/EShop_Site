using EShop_Site.Helpers;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace EShop_Site.Components;

public static class ResponseHandler
{
    public static async Task<ResponseHandlerResult<T>> HandleUniversalResponseAsync<T>(HttpResponseMessage response, bool responseCanBeNull = false) where T : class
    {
        var userResponse = await JsonHelper.GetTypeFromResponseAsync<UniversalResponse<T>>(response);
        
        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = userResponse.ErrorInfo ?? "";

            return new ResponseHandlerResult<T>(false);
        }
        
        MessageStorage.InfoMessage = userResponse.Info ?? "";

        if (!responseCanBeNull && userResponse.ResponseObject is null)
        {
            throw new Exception("Received null user object!");
        }

        return new ResponseHandlerResult<T>(result: userResponse.ResponseObject);
    }
    
    public static async Task<bool> HandleUniversalResponseAndGetStatusAsync(HttpResponseMessage response)
    {
        var userResponse = await JsonHelper.GetTypeFromResponseAsync<UniversalResponse>(response);
        
        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = userResponse.ErrorInfo ?? "";

            return false;
        }
        
        MessageStorage.InfoMessage = userResponse.Info ?? "";
        
        return true;
    }
}

public class ResponseHandlerResult<T> where T : class
{
    public bool IsSuccessful { get; set; }
    public T? Result { get; set; }

    public ResponseHandlerResult(bool isSuccessful = true, T? result = null)
    {
        IsSuccessful = isSuccessful;
        Result = result;
    }
}