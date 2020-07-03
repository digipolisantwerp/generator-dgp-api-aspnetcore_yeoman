using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace StarterKit.Shared.Swagger
{
    public class AddAuthorizationHeaderRequired : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor.FilterDescriptors.Any(x => x.Filter.GetType() == typeof(AuthorizeFilter))
                && context.ApiDescription.ActionDescriptor.FilterDescriptors.All(x => x.Filter.GetType() != typeof(AllowAnonymousFilter)))
            {
                operation.Parameters?.Add(new OpenApiParameter()
                {
                    Name = "Authorization",
                    Description = "JWT token",
                    Required = true,
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema
                    {
                      Type = "string",
                      Default = new OpenApiString("Bearer ")
                    }
                });
            }
        }
    }

    public class RemoveVersionFromParameter : IOperationFilter
    {
      public void Apply(OpenApiOperation operation, OperationFilterContext context)
      {
        //var versionParam = operation.Parameters?.Single(p => p.Name == "api-version");
        //operation.Parameters?.Remove(versionParam);
      }
    }

    public class ReplaceVersionWithExactValueInPath: IDocumentFilter
  {
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
      //swaggerDoc.Paths = (OpenApiPaths)swaggerDoc.Paths
      //  .ToDictionary(
      //    path => path.Key.Replace("v{version", swaggerDoc.Info.Version),
      //    path => path.Value);
    }
  }
}
