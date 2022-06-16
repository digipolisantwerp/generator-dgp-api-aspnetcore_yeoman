using System.ComponentModel.DataAnnotations;

namespace StarterKit.Entities
{
	/// <summary>
	/// These are the base classes for all entities
	/// The repositories have this class as constraint for entities that are persisted in the database.
	/// The default will have an integer type key but if desired you can specify a different type id.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EntityBase<T> : IEntityBase<T>
	{
		[Key] public T Id { get; set; }
	}

	public class EntityBase : IEntityBase<int>
	{
		[Key] public int Id { get; set; }
	}
}