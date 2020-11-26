using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StarterKit.DataAccess;
using StarterKit.DataAccess.Context;
using StarterKit.IntegrationTests.Shared.Attributes;

namespace StarterKit.IntegrationTests.Shared
{
  [AutoRollback]
  public class ControllerTestBaseMongo : IDisposable
  {
    protected string BasePath { get; set; }
    protected EntityContext DbContext { get; private set; }
    protected TestBaseFixture Fixture { get; private set; }

    public ControllerTestBaseMongo(TestBaseFixture fixture)
    {
      Fixture = fixture;
      DbContext = fixture.GetService<EntityContext>();
    }

    protected async Task<HttpResponseMessage> GetAsync(string path, bool checkSuccess = true)
    {
      return await SendAsync(HttpMethod.Get, path, checkSuccess);
    }

    protected async Task<HttpResponseMessage> PostAsync<TRequest>(string path, TRequest body, bool checkSuccess = true)
    {
      return await SendAsync(HttpMethod.Post, path, body, checkSuccess);
    }

    protected async Task<HttpResponseMessage> PatchAsync<TRequest>(string path, TRequest body, bool checkSuccess = true)
    {
      return await SendAsync(HttpMethod.Patch, path, body, checkSuccess);
    }

    protected async Task<HttpResponseMessage> PutAsync<TRequest>(string path, TRequest body, bool checkSuccess = true)
    {
      return await SendAsync(HttpMethod.Put, path, body, checkSuccess);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string path, bool checkSuccess = true)
    {
      return await SendAsync(HttpMethod.Delete, path, checkSuccess);
    }

    protected async Task<T> ParseResultAsync<T>(HttpResponseMessage response)
    {
      return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
    }

    private async Task<HttpResponseMessage> SendAsync<TRequest>(HttpMethod httpMethod, string path, TRequest body, bool checkSuccess)
    {
      var sbl = JsonConvert.SerializeObject(body,
        new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });


      var httpContent = new StringContent(sbl, Encoding.UTF8, "application/json");

      return await SendAsync(httpMethod, path, checkSuccess, httpContent);
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string path, bool checkSuccess, StringContent content = null)
    {
      using var apiRequest = new HttpRequestMessage(httpMethod, path);
      if (content != null)
      {
        apiRequest.Content = content;
      }

      var response = await Fixture.Client.SendAsync(apiRequest);

      if (checkSuccess)
      {
        response.IsSuccessStatusCode.Should().BeTrue();
      }

      return response;
    }

    public void Dispose()
    {
      foreach (var mock in Fixture.Mocks)
      {
        mock.Reset();
      }
    }
  }
}
