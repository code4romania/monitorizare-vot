using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace VotingIrregularities.Api.Extensions
{
    public class AddFileUploadParams : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!string.Equals(operation.OperationId, "ApiV1NoteAtaseazaPost", StringComparison.CurrentCultureIgnoreCase))
                return;

            operation.Consumes.Add("application/form-data");
            operation.Parameters = new IParameter[]
            {
                new NonBodyParameter
                {

                    Name = "file",
                    In = "formData",
                    Required = true,
                    Type = "file"
                }
            };
        }
    }
}
