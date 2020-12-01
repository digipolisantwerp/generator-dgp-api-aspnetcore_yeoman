using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StarterKit.DataAccess.Repositories;
using StarterKit.UnitTests.DataAccess._TestObjects;
using Xunit;

namespace StarterKit.UnitTests.DataAccess.Repositories
{
  public class GenericEntityRepositoryTests
  {
    private InMemoryContext _context;
    private IRepository<Foo, int> _fooRepository;

    public GenericEntityRepositoryTests()
    {
      _fooRepository = new GenericEntityRepository<Foo>(null);
      _context = InMemoryContext.Create();
      _fooRepository.SetContext(_context);
    }

    private IEnumerable<Foo> GetEntities(int count = 10)
    {
      var result = new List<Foo>();
      // Add entities to context
      for (var i = 1; i <= count; i++)
      {
        var foo = new Foo()
        {
          Id = i
        };

        result.Add(foo);
      }

      return result;
    }

    private void AddEntitiesToContext(int count = 10)
    {
      // Add entities to context
      _context.AddRange(GetEntities(count));

      _context.SaveChanges();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void GetAllDoesNotReturnNull(int totalEntities)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.GetAll();

      // Assert
      Assert.NotNull(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void GetAllReturnsCorrectNumberOfEntities(int totalEntities)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.GetAll();

      // Assert
      Assert.Equal(totalEntities, result.Count());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void GetAllReturnsAllEntitiesInContext(int totalEntities)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.GetAll();

      // Assert
      for (int i = 1; i <= totalEntities; i++)
        Assert.Equal(1, result.Count(x => x.Id == i));
    }

    [Theory]
    [InlineData(10)]
    public void GetAllSortedByIdReturnsAllEntitiesInCorrectOrder(int totalEntities)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.GetAll();

      // Assert
      var i = 0;
      foreach (var entity in result)
      {
        i++;
        Assert.Equal(i, entity.Id);
      }
    }

    [Theory]
    [InlineData(10)]
    public void GetAllReverseSortedByIdReturnsAllEntitiesInCorrectOrder(int totalEntities)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.GetAll("-id");

      // Assert
      var i = totalEntities;
      foreach (var entity in result)
      {
        Assert.Equal(i, entity.Id);
        i--;
      }
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(3, 4)]
    public void GetReturnsNullWhenEntityDoesNotExistInContext(int totalEntities, int id)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.Get(id);

      // Assert
      Assert.Null(result);
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(3, 3)]
    public void GetDoesNotReturnNullWhenEntityExistsInContext(int totalEntities, int id)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.Get(id);

      // Assert
      Assert.NotNull(result);
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(3, 3)]
    public void GetReturnsSingleEntityWithProvidedId(int totalEntities, int id)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.Get(id);

      // Assert
      Assert.Equal(id, result.Id);
    }

    [Theory]
    [InlineData(10, 0, 5)]
    [InlineData(10, 5, 5)]
    [InlineData(10, 10, 5)]
    public void GetPageSortedByIdDoesNotReturnNull(int totalEntities, int rowIndex, int pageSize)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.GetPage(rowIndex, pageSize);

      // Assert
      Assert.NotNull(result);
    }

    [Theory]
    [InlineData(10, 0, 5)]
    [InlineData(10, 5, 5)]
    [InlineData(10, 10, 5)]
    [InlineData(8, 5, 5)]
    public void GetPageSortedByIdReturnsCorrectNumberOfEntities(int totalEntities, int rowIndex, int pageSize)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);
      var minId = rowIndex + 1;
      var maxId = Math.Min(totalEntities, minId + pageSize - 1);
      var expectedNumberOfEntities = Math.Max(0, maxId - minId + 1);

      // Act
      var result = _fooRepository.GetPage(rowIndex, pageSize);

      // Assert
      Assert.Equal(expectedNumberOfEntities, result.Count());
    }

    [Theory]
    [InlineData(10, 0, 5)]
    [InlineData(10, 5, 5)]
    [InlineData(10, 10, 5)]
    [InlineData(8, 5, 5)]
    public void GetPageSortedByIdReturnsEntitiesWithCorrectIds(int totalEntities, int rowIndex, int pageSize)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      var minId = rowIndex + 1;
      var maxId = Math.Min(totalEntities, minId + pageSize - 1);

      // Act
      var result = _fooRepository.GetPage(rowIndex, pageSize);

      // Assert
      for (int i = minId; i <= maxId; i++)
        Assert.Equal(1, result.Count(x => x.Id == i));
    }

    [Theory]
    [InlineData(10, 1)]
    [InlineData(10, 5)]
    [InlineData(10, 10)]
    public void AnyByIdReturnsTrueForExistingRecord(int totalEntities, int id)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.Any(x => x.Id == id);

      // Assert
      Assert.True(result);
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(10, 11)]
    public void AnyByIdReturnsFalseForNonExistingRecord(int totalEntities, int id)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.Any(x => x.Id == id);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void RemoveOne()
    {
      AddEntitiesToContext();
      _fooRepository.Remove(_fooRepository.Get(1));
      _context.SaveChanges();

      Assert.Equal(9, _context.Foos.Count());
      Assert.False(_context.Foos.Any(f => f.Id == 1));
    }

    [Fact]
    public void RemoveOneById()
    {
      AddEntitiesToContext();
      _fooRepository.Remove(1);
      _context.SaveChanges();

      Assert.Equal(9, _context.Foos.Count());
      Assert.False(_context.Foos.Any(f => f.Id == 1));
    }

    [Fact]
    public void RemoveRangeById()
    {
      AddEntitiesToContext();
      _fooRepository.RemoveBatch(new []{1, 2, 3, 4, 5});
      _context.SaveChanges();

      Assert.Equal(5, _context.Foos.Count());
      Assert.False(_context.Foos.Any(f =>
        f.Id == 1
        || f.Id == 2
        || f.Id == 3
        || f.Id == 4
        || f.Id == 5));
    }

    [Fact]
    public void RemoveRange()
    {
      AddEntitiesToContext();
      var listToRemove = _fooRepository.GetAll();

      _fooRepository.RemoveBatch(listToRemove);
      _context.SaveChanges();

      Assert.Equal(0, _context.Foos.Count());
    }

    [Fact]
    public void AddRange()
    {
      _fooRepository.AddBatch(GetEntities());
      _context.SaveChanges();

      Assert.Equal(10, _context.Foos.Count());
    }

    [Fact]
    public void UpdateRange()
    {
      AddEntitiesToContext();
      var entitiesToUpdate = _context.Foos.Take(5);
      foreach (var entity in entitiesToUpdate)
      {
        entity.Name = "Updated";
      }

      _fooRepository.UpdateBatch(entitiesToUpdate);
      _context.SaveChanges();

      Assert.Equal(5, _context.Foos.Count(f => f.Name == "Updated"));
    }
  }
}
