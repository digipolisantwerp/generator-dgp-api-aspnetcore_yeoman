using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace StarterKit.Shared.Swagger
{
    public class RemoveSyncRootParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters?.Remove(operation.Parameters?.Where(x => x.Name == "SyncRoot").FirstOrDefault());
        }
    }
}
