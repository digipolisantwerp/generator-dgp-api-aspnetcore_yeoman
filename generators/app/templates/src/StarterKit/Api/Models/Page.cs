using System.Collections.Generic;

namespace StarterKit.Api.Models
{
	public class Page<TModel>
	{
		public IEnumerable<TModel> Data { get; set; }
		public long TotalCount { get; set; }
	}
}