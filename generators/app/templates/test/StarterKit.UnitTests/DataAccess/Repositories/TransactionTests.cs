using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StarterKit.DataAccess.Repositories;
using StarterKit.UnitTests.DataAccess._TestObjects;
using Xunit;

namespace StarterKit.UnitTests.DataAccess.Repositories
{
  public class TransactionTests
  {
    private SqlLiteContext _context;
    private IRepository<Foo, int> _fooRepository;

    public TransactionTests()
    {
      _fooRepository = new GenericEntityRepository<Foo>(null);

      _context = new SqlLiteContext();

      //delete the old database
      if (File.Exists("TestDatabase.db"))
      {
        File.Delete("TestDatabase.db");
      }
      //migrate the database
      _context.Database.Migrate();

      _fooRepository.SetContext(_context);
    }

    private IEnumerable<Foo> GetEntities(int start = 1, int count = 10)
    {
      var result = new List<Foo>();
      // Add entities to context
      for (var i = start; i < start+count; i++)
      {
        var foo = new Foo()
        {
          Id = i
        };

        result.Add(foo);
      }

      return result;
    }

    private void AddEntitiesToContext(int start = 1, int count = 10)
    {
      // Add entities to context
      _context.AddRange(GetEntities(start, count));

      _context.SaveChanges();
    }

    [Fact]
    public void ShouldRollback()
    {
      try
      {
        _context.BeginTransaction();

        AddEntitiesToContext();
        AddEntitiesToContext();

        _context.Commit();
      }
      catch (Exception)
      {
        _context.Rollback();
      }

      Assert.Equal(0, _context.Foos.Count());
    }

    [Fact]
    public void ShouldCommitOnSuccess()
    {
      try
      {
        _context.BeginTransaction();

        AddEntitiesToContext();
        AddEntitiesToContext(11);

        _context.Commit();
      }
      catch (Exception)
      {
        _context.Rollback();
      }

      Assert.Equal(20, _context.Foos.Count());
    }

  }
}
