using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace StarterKit.Shared.Swagger
{
    public class AddAuthorizationHeaderRequired : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor.FilterDescriptors.Any(x => x.Filter.GetType() == typeof(AuthorizeFilter))
                && !context.ApiDescription.ActionDescriptor.FilterDescriptors.Any(x => x.Filter.GetType() == typeof(AllowAnonymousFilter)))
            {
                operation.Parameters?.Add(new NonBodyParameter()
                {
                    Name = "Authorization",
                    Type = "string",
                    Required = true,
                    In = "header"
                });
            }
        }
    }
}
