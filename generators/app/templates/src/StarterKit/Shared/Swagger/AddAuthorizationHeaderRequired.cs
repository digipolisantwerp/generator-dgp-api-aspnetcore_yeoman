using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
}
