using EShop_Site.Components;

namespace EShop_Site.Services.Abstract;

public interface IHttpClientService
{
    public Task<HttpResponseMessage> SendRequestAsync(RestRequestForm requestForm);
}