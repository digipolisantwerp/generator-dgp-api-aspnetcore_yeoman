using AutoMapper;
using Xunit;
using StarterKit.IntegrationTests.Shared;

namespace StarterKit.IntegrationTests.AutoMapper
{
  [Collection(TestConstants.Collections.ControllerTests)]
  public class AutoMapperTests : IClassFixture<TestBaseFixture>
  {
    private readonly IMapper _mapper;

    public AutoMapperTests(TestBaseFixture fixture)
    {
      _mapper = fixture.GetService<IMapper>();
    }

    [Fact]
    public void Mapper_AssertConfigurationIsValid_Expect_No_Exceptions()
    {
      _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
  }
}
