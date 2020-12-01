using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StarterKit.DataAccess.Context;
using StarterKit.DataAccess.Repositories;

namespace StarterKit.Startup
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddDataAccess<TEntityContext>(this IServiceCollection services)
      where TEntityContext : Context<TEntityContext>
    {
      RegisterDataAccess<TEntityContext>(services);
      return services;
    }

    private static void RegisterDataAccess<TEntityContext>(IServiceCollection services)
      where TEntityContext : Context<TEntityContext>
    {
      services.TryAddScoped<IContext, TEntityContext>();
      services.TryAddTransient(typeof(IRepository<,>), typeof(GenericEntityRepository<>));
    }

    private static void ValidateMandatoryField(string field, string fieldName)
    {
      if (field == null) throw new ArgumentNullException(fieldName, $"{fieldName} cannot be null.");
      if (field.Trim() == String.Empty) throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);
    }

    private static void ValidateMandatoryField(object field, string fieldName)
    {
      if (field == null) throw new ArgumentNullException(fieldName, $"{fieldName} cannot be null.");
    }
  }
}
