using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SBSC_Store.Swagger;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IEnumerable<IFormFile>))
            .ToList();

        // Also check for [FromForm] complex types containing IFormFile properties
        var bodyParam = context.MethodInfo.GetParameters()
            .FirstOrDefault(p => !fileParams.Contains(p) && p.GetCustomAttributes(true).Any(a => a.GetType().Name == "FromFormAttribute"));
        var modelFileProps = new Dictionary<string, OpenApiSchema>();
        if (bodyParam != null)
        {
            var props = bodyParam.ParameterType.GetProperties()
                .Where(pi => pi.PropertyType == typeof(IFormFile) || pi.PropertyType == typeof(IEnumerable<IFormFile>));
            foreach (var pi in props)
                modelFileProps[pi.Name] = new OpenApiSchema { Type = "string", Format = "binary" };
        }

        if (!fileParams.Any() && !modelFileProps.Any())
            return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = fileParams
                            .ToDictionary(p => p.Name, p => (OpenApiSchema)new OpenApiSchema { Type = "string", Format = "binary" })
                            .Concat(modelFileProps)
                            .ToDictionary(k => k.Key, v => v.Value)
                    }
                }
            }
        };
    }
}
