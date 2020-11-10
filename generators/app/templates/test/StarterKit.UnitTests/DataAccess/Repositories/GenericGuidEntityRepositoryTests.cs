using System;
using System.Collections.Generic;
using System.Linq;
using StarterKit.DataAccess.Repositories;
using StarterKit.UnitTests.DataAccess._TestObjects;
using Xunit;

namespace StarterKit.UnitTests.DataAccess.Repositories
{
  public class GenericGuidEntityRepositoryTests
  {
    private readonly InMemoryContext _context;
    private readonly IRepository<FooGuid, Guid> _fooRepository;
    private readonly Dictionary<int, Guid> _keys;

    public GenericGuidEntityRepositoryTests()
    {
      _fooRepository = new GenericEntityRepository<FooGuid, Guid>(null);
      _context = InMemoryContext.Create();
      ((IRepositoryInjection) _fooRepository).SetContext(_context);

      // add sorted GUID dictionary so we can easily access all ID's and verify the correct order
      _keys = new Dictionary<int, Guid>
      {
        { 1, new Guid("366f2b72-c8df-4dbc-9b54-1940a897cab3")},
        { 2, new Guid("54a869b4-f374-48de-bbb6-cefee82404eb")},
        { 3, new Guid("5612b1bc-9f65-4908-ae6a-5ad4ca9f592f")},
        { 4, new Guid("6e4990c9-f4d8-4ea9-8b1f-7fc826582606")},
        { 5, new Guid("ac41d795-f7f2-453c-a6d0-94c247f5e15b")},
        { 6, new Guid("af11a46f-84d1-44fa-aa4f-957ac021621f")},
        { 7, new Guid("b9fd0f3c-8630-4f28-aa43-65f5f2e09bc9")},
        { 8, new Guid("c2f916a3-3cc2-40c2-9958-d5f0b3350805")},
        { 9, new Guid("ca45d769-55c9-41d6-96fe-9aefa090f1ec")},
        { 10, new Guid("d52dd4ef-025c-4bbf-937f-d091bc7d02ae")}
        };
    }

    private void AddEntitiesToContext(int count = 10)
    {
      // Add entities to context
      for (var i = 1; i <= count; i++)
      {
        var foo = new FooGuid
        {
          Id = _keys[i]
        };

        _context.FooGuids.Add(foo);
      }

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
        Assert.Equal(1, result.Count(x => x.Id == _keys[i]));
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
        Assert.Equal(_keys[i], entity.Id);
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
        Assert.Equal(_keys[i], entity.Id);
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
      var result = _fooRepository.Get(_keys[id]);

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
    [InlineData(10, "6e20d2e9-ee4a-453e-a401-2f6c5525a4af")]
    [InlineData(10, "9ddd8d71-37a1-4bea-aecb-58ef3342f7b1")]
    public void AnyByIdReturnsFalseForNonExistingRecord(int totalEntities, Guid id)
    {
      // Arrange
      AddEntitiesToContext(totalEntities);

      // Act
      var result = _fooRepository.Any(x => x.Id == id);

      // Assert
      Assert.False(result);
    }
  }
}
