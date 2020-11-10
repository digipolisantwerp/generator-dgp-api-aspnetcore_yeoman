using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Repositories;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class FooGuidRepository : EntityRepositoryBase<InMemoryContext, FooGuid, Guid>, IFooRepository
  {
    public FooGuidRepository(ILogger<StarterKit.DataAccess.DataAccess> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory)
    {
    }
  }
}
