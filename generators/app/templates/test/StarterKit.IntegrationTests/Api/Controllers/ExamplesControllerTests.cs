using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using StarterKit.Api.Models;
using StarterKit.IntegrationTests.Shared;
using Xunit;

namespace StarterKit.IntegrationTests.Api.Controllers
{
  [Collection(TestConstants.Collections.ControllerTests)]
  public class ExamplesControllerTests : ControllerTestBase
  {
    public ExamplesControllerTests(TestBaseFixture fixture) : base(fixture)
    {
      BasePath = "v1/examples";
    }

    [Fact]
    public async void GetExistingExample_ShouldReturnExample()
    {
      //Arrange
      var url = $"{BasePath}/1";

      //Act
      var response = await GetAsync(url);

      //Assert
      Assert.Equal(ReasonPhrases.GetReasonPhrase(StatusCodes.Status200OK), response.ReasonPhrase);

      var example = await ParseResultAsync<Example>(response);

      Assert.Equal(1, example.Id);
      Assert.Equal("Peter Parker", example.Name);
    }

    [Fact]
    public async void GetNonExistingExample_ShouldReturnNotFound()
    {
      //Arrange
      var url = $"{BasePath}/8";

      //Act
      var response = await GetAsync(url, false);

      //Assert
      Assert.Equal(ReasonPhrases.GetReasonPhrase(StatusCodes.Status404NotFound), response.ReasonPhrase);
    }
  }
}
