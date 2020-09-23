using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StarterKit.Shared.Swagger
{
    public class LowerCaseQueryAndBodyParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;

            foreach (var parameter in operation.Parameters.Where(x => x.In != null && (x.In == ParameterLocation.Query)))
            {
              parameter.Name = parameter.Name.ToLowerInvariant();
            }
        }
    }
}
