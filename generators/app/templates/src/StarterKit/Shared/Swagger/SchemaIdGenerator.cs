using System;
using System.Linq;
using System.Reflection;

namespace StarterKit.Shared.Swagger
{
    public static class SchemaIdGenerator
    {
        public static String GenerateSchemaId(Type type)
        {
            if (type.GetTypeInfo().GetCustomAttributes(typeof(SwashbuckleCustomSchemaId), true).FirstOrDefault() is SwashbuckleCustomSchemaId swashbuckleCustomSchemaIdAttribute)
                return swashbuckleCustomSchemaIdAttribute.SchemaId;
            var arguments = type.GetGenericArguments();
            if (arguments.Any()) return $"{type.Name.Split('`').First()}[{String.Join(",", arguments.Select(z => z.Name))}]";
            return type.Name;
        }
    }
}
