using System;

namespace StarterKit.Shared.Swagger
{
    //use this attribute to prevent duplicate schema id's in swagger definition for classes with identical names in different namespaces
    public class SwashbuckleCustomSchemaId : Attribute
    {
        public string SchemaId { get; set; }
    }
}
