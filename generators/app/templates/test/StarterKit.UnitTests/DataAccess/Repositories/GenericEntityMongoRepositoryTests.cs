using System;
using System.Collections.Generic;
using System.Linq;
using StarterKit.DataAccess.Options;
using StarterKit.DataAccess.Repositories;
using StarterKit.UnitTests.DataAccess._TestObjects;
using MongoDB.Driver;
using Xunit;

namespace StarterKit.UnitTests.DataAccess.Repositories
{
  public class GenericEntityMongoRepositoryTests
  {
    private MongoContext _context;
    private IRepositoryMongo<FooMongo, string> _fooRepository;
    private Dictionary<int, string> _keys;

    public GenericEntityMongoRepositoryTests()
    {
      _fooRepository = new GenericEntityRepositoryMongo<FooMongo, string>(null);

      var settings = new DataAccessSettingsMongo
      {
        Host = "localhost",
        Port = "27017",
        DbName = "StarterKitRepositoryTest"
      };

      _context = new MongoContext(settings);

      _context.Foos.DeleteMany(_ => true);
      _fooRepository.SetContext(_context);
      _keys = new Dictionary<int, string>();
    }

    private IEnumerable<FooMongo> GetEntities(int count = 10)
    {
      var result = new List<FooMongo>();
      // Add entities to context
      for (var i = 1; i <= count; i++)
      {
        var foo = new FooMongo()
        {
          Name = i.ToString()
        };

        result.Add(foo);
      }

      return result;
    }

    private void AddEntitiesToContext(int count = 10)
    {
      // Add entities to context
      var entities = GetEntities(count);
      if (entities.Any())
      {
        _context.Foos.InsertMany(entities);

        //set all ID's to the key
        foreach (var e in entities)
        {
          _keys.Add(int.Parse(e.Name), e.Id);
        }
      }

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
        Assert.Equal(1, result.Count(x => x.Name == i.ToString()));
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
        Assert.Equal(i.ToString(), entity.Name);
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
        Assert.Equal(i.ToString(), entity.Name);
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

      _keys.TryGetValue(id, out var sId);
      // Act
      var result = _fooRepository.Get(sId);

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
      var result = _fooRepository.Get(_keys[id]);

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
      var result = _fooRepository.Get(_keys[id]);

      // Assert
      Assert.Equal(_keys[id], result.Id);
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
        Assert.Equal(1, result.Count(x => x.Id == _keys[i]));
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
      var result = _fooRepository.Any(x => x.Id == _keys[id]);

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

      _keys.TryGetValue(id, out var sId);

      // Act
      var result = _fooRepository.Any(x => x.Id == sId);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void RemoveOne()
    {
      AddEntitiesToContext();
      _fooRepository.Remove(_fooRepository.Get(_keys[1]));

      Assert.Equal(9, _context.Foos.CountDocuments(_ => true));
      Assert.Null(_context.Foos.FindSync(f => f.Id == _keys[1]).SingleOrDefault());
    }

    [Fact]
    public void RemoveOneById()
    {
      AddEntitiesToContext();
      _fooRepository.Remove(_keys[1]);

      Assert.Equal(9, _context.Foos.CountDocuments(_ => true));
      Assert.False(_context.Foos.FindSync(f => f.Id == _keys[1]).Any());
    }

    [Fact]
    public void RemoveRangeById()
    {
      AddEntitiesToContext();
      _fooRepository.RemoveBatch(new []{_keys[1], _keys[2], _keys[3], _keys[4], _keys[5]});

      Assert.Equal(5, _context.Foos.CountDocuments(_ => true));
      Assert.False(_context.Foos.FindSync(f =>
        f.Id == _keys[1]
        || f.Id == _keys[2]
        || f.Id == _keys[3]
        || f.Id == _keys[4]
        || f.Id == _keys[5]).Any());
    }

    [Fact]
    public void RemoveRange()
    {
      AddEntitiesToContext();
      var listToRemove = _fooRepository.GetAll();

      _fooRepository.RemoveBatch(listToRemove);

      Assert.Equal(0, _context.Foos.CountDocuments(_ => true));
    }

    [Fact]
    public void AddRange()
    {
      _fooRepository.AddBatch(GetEntities());
      Assert.Equal(10, _context.Foos.CountDocuments(_ => true));
    }

    [Fact]
    public void UpdateRange()
    {
      AddEntitiesToContext();
      var entitiesToUpdate = _context.Foos.AsQueryable().Take(5).ToList();
      foreach (var entity in entitiesToUpdate)
      {
        entity.Name = "Updated";
      }

      _fooRepository.UpdateBatch(entitiesToUpdate);

      Assert.Equal(5, _context.Foos.CountDocuments(f => f.Name == "Updated"));
    }

  }
}
