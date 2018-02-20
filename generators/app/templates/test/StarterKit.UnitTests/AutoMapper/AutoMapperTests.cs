using StarterKit.IntegrationTests.Helpers;
using AutoMapper;
using Xunit;

namespace StarterKit.UnitTests.AutoMapper
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