using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StarterKit.Shared.Swagger
{
	public class SetOperationDescription : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			if (string.IsNullOrEmpty(operation.Description))
				operation.Description = operation.Summary;
		}
	}
}