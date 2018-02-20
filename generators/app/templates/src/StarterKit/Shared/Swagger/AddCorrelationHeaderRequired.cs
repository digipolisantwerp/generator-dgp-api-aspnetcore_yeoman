using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StarterKit.Shared.Swagger
{
    public class AddCorrelationHeaderRequired : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Parameters?.Add(new NonBodyParameter()
            {
                Name = "Dgp-Correlation",
                Description = "Correlation info",
                Type = "string",
                Required = context.ApiDescription.ActionDescriptor.RouteValues["Controller"] != "Status",
                In = "header"
            });
        }
    }
}
