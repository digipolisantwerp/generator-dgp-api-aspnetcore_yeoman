using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using AutoMapper;
using Digipolis.DataAccess.Entities;
using Digipolis.Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.TestCorrelator;
using StarterKit.Api.Controllers;
using StarterKit.DataAccess;
using StarterKit.Logging;
using StarterKit.Shared.Options;
using Xunit;

namespace StarterKit.UnitTests.Logger
{
  public class DbContextTest
  {

    [Fact]
    public void ShouldLogDbContext()
    {
      var entityContextMock = TestContext.GetTestContext();

      Log.Logger = new LoggerConfiguration()
        .WriteTo
        .TestCorrelator()
        .CreateLogger();


      using (TestCorrelator.CreateContext())
      {

        // var serviceProvider = new ServiceCollection()
        //   .AddEntityFrameworkInMemoryDatabase()
        //   .BuildServiceProvider();
        //
        // var builder = new DbContextOptionsBuilder<TestContext>();
        // builder.UseInMemoryDatabase().UseInternalServiceProvider(serviceProvider);
        //
        // var entityContextMock = new Mock<TestContext>();

        // entityContextMock.Setup(x => x.Set<TestEntity>())
        //   .Returns((DbSet<TestEntity>) new List<TestEntity>
        //   {
        //     new TestEntity {Id = 1, Name = "Test"}
        //   }.AsQueryable());



        entityContextMock.TestEntities.Add(new TestEntity {Id = 1, Name = "Test"});

        entityContextMock.SaveChanges();


      }

    }


  }

}

public class TestEntity : EntityBase
{
  public string Name { get; set; }
}

public class TestContext : EntityContext
{
  public TestContext(DbContextOptions<EntityContext> options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
  {
  }

  public DbSet<TestEntity> TestEntities { get; set; } = GetQueryableMockDbSet(new List<TestEntity>
  {
    new TestEntity {Id = 1, Name = "Test"}
  });

  public static TestContext GetTestContext()
  {
    // Create a fresh service provider, and therefore a fresh
    // InMemory database instance.
    var serviceProvider = new ServiceCollection()
      .AddLoggingEngine()
      .AddEntityFrameworkInMemoryDatabase()
      .BuildServiceProvider();


    // Create a new options instance telling the context to use an
    // InMemory database and the new service provider.
    var builder = new DbContextOptionsBuilder<EntityContext>();
    builder.UseInMemoryDatabase("TestDatabase")
      .UseInternalServiceProvider(serviceProvider);

    return new TestContext(builder.Options, serviceProvider.GetRequiredService<ILoggerFactory>());
  }

  private static DbSet <T> GetQueryableMockDbSet<T>(List <T> sourceList) where T: class
  {
    var queryable = sourceList.AsQueryable();
    var dbSet = new Mock<DbSet<T>>();
    dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
    dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
    dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
    dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
    dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));
    return dbSet.Object;
  }

}
