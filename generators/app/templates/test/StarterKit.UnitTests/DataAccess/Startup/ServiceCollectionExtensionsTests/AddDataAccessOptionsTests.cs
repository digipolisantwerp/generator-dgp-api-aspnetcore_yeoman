using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StarterKit.DataAccess.Repositories;
using StarterKit.Startup;
using StarterKit.UnitTests.DataAccess._TestObjects;
using Xunit;

namespace StarterKit.UnitTests.DataAccess.Startup.ServiceCollectionExtensionsTests
{
  public class AddDataAccessOptionsTests
  {

    [Fact]
    private void GenericRepositoryIsRegisteredAsTransient()
    {
      var services = new ServiceCollection();

      services.AddDataAccess<TestContext>();

      var registrations = services.Where(sd => sd.ServiceType == typeof(IRepository<,>)
                                               && sd.ImplementationType == typeof(GenericEntityRepository<>))
        .ToArray();
      Assert.Single(registrations);
      Assert.Equal(ServiceLifetime.Transient, registrations[0].Lifetime);
    }

  }
}
