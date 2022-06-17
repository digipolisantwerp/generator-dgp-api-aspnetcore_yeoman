using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Repositories;
using System;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
	public class FooGuidRepository : EntityRepositoryBase<InMemoryContext, FooGuid, FooGuidRepository, Guid>, IFooRepository
  {
    public FooGuidRepository(ILogger<FooGuidRepository> logger, InMemoryContext context) : base(logger, context)
    {
    }
  }
}
