using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StarterKit.Shared.Swagger
{
    public class AddCorrelationHeaderRequired : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
          operation.Parameters?.Add(new OpenApiParameter()
          {
            Name = "Dgp-Correlation",
            Description = "Correlation info",
            Required = context.ApiDescription.ActionDescriptor.RouteValues["Controller"] != "Status",
            In = ParameterLocation.Header,
            Schema = new OpenApiSchema { Type = "string" }
          });
      }
  }
}
