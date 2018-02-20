using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Xunit;

namespace StarterKit.UnitTests.Helpers
{
  public abstract class ServerTestBase : IClassFixture<ServerFixture>
  {
    protected readonly TestServer _server;
    protected readonly HttpClient _client;

    protected ServerTestBase(ServerFixture serverFixture)
    {
      _server = serverFixture.Server;
      _client = serverFixture.Client;
    }
  }
}
