using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StarterKit.Entities
{

  /// <summary>
  /// These are the base classes for all entities
  /// The repositories have this class as constraint for entities that are persisted in the database.
  /// The default will have an integer type key but if desired you can specify a different type id.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class EntityBaseMongo<T> : IEntityBase<T>
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public T Id { get; set; }
  }

  public class EntityBaseMongo : IEntityBase<string>
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
  }
}
