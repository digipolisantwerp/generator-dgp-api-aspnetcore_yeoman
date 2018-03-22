using AutoMapper;
using Xunit;
using StarterKit.IntegrationTests.Helpers;

namespace StarterKit.IntegrationTests.AutoMapper
{
  [Collection("Integration tests collection")]
  public class AutoMapperTests : ServerTestBase
  {
    public AutoMapperTests(ServerFixture serverFixture) : base(serverFixture)
    {
    }

    [Fact]
    public void Mapper_AssertConfigurationIsValid_Expect_No_Exceptions()
    {

      Mapper.AssertConfigurationIsValid();
    }
  }
}
