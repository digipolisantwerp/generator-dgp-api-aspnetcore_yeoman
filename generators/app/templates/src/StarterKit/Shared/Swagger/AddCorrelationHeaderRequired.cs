using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StarterKit.Shared.Swagger
{
  public class AddCorrelationHeaderRequired : IOperationFilter
  {
    private readonly IHostEnvironment _environment;

    public AddCorrelationHeaderRequired(IServiceProvider serviceProvider)
    {
      _environment = serviceProvider.GetRequiredService<IHostEnvironment>();
    }
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      operation.Parameters?.Add(new OpenApiParameter()
      {
        Name = "Dgp-Correlation",
        Description = "Correlation info",
        Required = context.ApiDescription.ActionDescriptor.RouteValues["Controller"] != "Status" && !_environment.IsDevelopment(),
        In = ParameterLocation.Header,
        Schema = new OpenApiSchema { Type = "string" }
      });
    }
  }
}
