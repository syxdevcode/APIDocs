using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using RazorEngine;
using RazorEngine.Templating;
using System.Linq;

namespace APIDocs.Helper
{
    public class T4Helper
    {
        private static OpenApiComponents components;

        public static string GeneritorSwaggerHtml(string templatePath, OpenApiDocument model)
        {
            //foreach (var item in model.Paths)
            //{
            //    var obj = item.Value.Operations.FirstOrDefault();
            //    if (obj.Key.ToString() == "Post")
            //    {

            //        var response = obj.Value.Responses.FirstOrDefault();

            //        if (response.Value.Content != null && response.Value.Content.Count > 0)
            //        {
            //            foreach (var t in response.Value.Content)
            //            {
            //                var reference = t.Value.Schema.Reference;

            //                if (reference != null)
            //                {
            //                    var schema = GetOpenApiSchemaInfo(reference.Id);

            //                    if (schema != null)
            //                    {
            //                        foreach (var param in schema.Properties)
            //                        {
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //    }
            //}

            components = model.Components;
            var template = System.IO.File.ReadAllText(templatePath);
            var result = Engine.Razor.RunCompile(template, "SwaggerDocs", typeof(OpenApiDocument), model);
            //var result = Engine.Razor.RunCompile(new LoadedTemplateSource(template, templatePath), typeof(SwaggerDocument), null, model);
            return result;

        }

        public static OpenApiSchema GetOpenApiSchemaInfo(string schemaKeyId)
        {
            var obj = components.Schemas.FirstOrDefault(o => o.Key == schemaKeyId);
            return obj.Value;
        }

        public static OpenApiMediaType GetRequestBodyByJoson(IDictionary<string, OpenApiMediaType> param)
        {
            var request = param.FirstOrDefault(o => o.Key.ToString() == "application/json");
            return request.Value;
        }
    }
}