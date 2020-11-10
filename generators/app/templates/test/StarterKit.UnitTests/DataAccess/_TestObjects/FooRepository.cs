using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Repositories;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class FooRepository : EntityRepositoryBase<InMemoryContext, Foo>, IFooRepository
  {
    public FooRepository(ILogger<StarterKit.DataAccess.DataAccess> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory)
    {
    }
  }
}
