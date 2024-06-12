using System.Net;
using System.Net.Http.Headers;
using System.Text;
using EShop_Site.Components;
using EShop_Site.Services.Abstract;

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
        using (var client = _clientFactory.CreateClient("MyApiClient"))
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
                    return response;
                }
            }
            catch (Exception ex) when (ex.Message.Contains("actively refused"))
            {
                await LogMachine.LogRequestToJson(
                    "The server is not responding");
                return new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    Content = new StringContent(
                        "No connection could be made because the target machine actively refused it.")
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}