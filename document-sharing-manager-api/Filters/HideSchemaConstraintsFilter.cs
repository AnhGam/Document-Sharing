using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace document_sharing_manager_api.Filters
{
    /// <summary>
    /// Filter to remove schema constraints (minLength, maxLength, pattern, etc.) from the OpenAPI documentation.
    /// This keeps the documentation clean while maintaining validation logic in the code.
    /// </summary>
    public class HideSchemaConstraintsFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema == null) return;

            // Clear constraints from the main schema
            ClearConstraints(schema);

            // Clear constraints from all properties if it's an object
            if (schema.Properties != null)
            {
                foreach (var property in schema.Properties.Values)
                {
                    ClearConstraints(property);
                }
            }
        }

        private static void ClearConstraints(OpenApiSchema schema)
        {
            schema.MinLength = null;
            schema.MaxLength = null;
            schema.Pattern = null;
            schema.Minimum = null;
            schema.Maximum = null;
            schema.ExclusiveMinimum = null;
            schema.ExclusiveMaximum = null;
            schema.MultipleOf = null;
            schema.MinItems = null;
            schema.MaxItems = null;
            schema.UniqueItems = null;
            schema.MinProperties = null;
            schema.MaxProperties = null;
        }
    }
}
