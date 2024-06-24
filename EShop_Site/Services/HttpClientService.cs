using System.Net;
using System.Net.Http.Headers;
using System.Text;
using EShop_Site.Components;
using EShop_Site.Services.Abstract;
using Newtonsoft.Json;
using SharedLibrary.Responses;

namespace EShop_Site.Services;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _clientFactory;
    private const string BlUrl = "https://localhost:44301/";

    public HttpClientService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(RestRequestForm requestForm)
    {
        using (var client = _clientFactory.CreateClient("TradeWaveApiClient"))
        {
            try
            {
                var request = new HttpRequestMessage(requestForm.RequestMethod, BlUrl + requestForm.EndPoint);

                if (requestForm.JsonData != null)
                {
                    request.Content = new StringContent(requestForm.JsonData, Encoding.UTF8, "application/json");
                }

                if (!string.IsNullOrWhiteSpace(requestForm.Token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", requestForm.Token);
                }

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    await LogMachine.LogRequestToJson("Successful communication with the database server");
                    return response;
                }
                else
                {
                    await LogMachine.LogRequestToJson(
                        "Communication with the database server occurred, but an unsuccessful action code was received");
                    Console.WriteLine(
                        $"The query returned an error code: {response.StatusCode}. Message: {await response.Content.ReadAsStringAsync()}");
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        return response;
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadGateway)
                        {
                            Content = new StringContent(
                                JsonConvert.SerializeObject(new UniversalResponse(
                                    "An unexpected error occurred on the server. Please try again! " +
                                    "If the problem persists, please contact support.")))
                        };
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                await LogMachine.LogRequestToJson($"HTTP request error: {httpEx.Message}");
                Console.WriteLine($"HTTP request error: {httpEx.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new UniversalResponse(
                            "An unexpected error occurred while trying to access the server. Please try again! " +
                            "If the problem persists, please contact support.")))
                };
            }
            catch (TaskCanceledException taskEx)
            {
                await LogMachine.LogRequestToJson("Request was canceled (possibly due to timeout).");
                Console.WriteLine("Request was canceled (possibly due to timeout).");
                return new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new UniversalResponse("The server's not responding")))
                };
            }
            catch (Exception ex)
            {
                await LogMachine.LogRequestToJson($"Unexpected error: {ex.Message}");
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}