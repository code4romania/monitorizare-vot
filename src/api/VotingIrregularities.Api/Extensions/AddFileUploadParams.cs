using System;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VotingIrregularities.Api.Extensions
{
    /// <inheritdoc />
    public class AddFileUploadParams : IOperationFilter
    {
        /// <inheritdoc />
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
