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
            if (operation.Tags[0] == "Observer" && operation.OperationId == "Import" ||
                operation.Tags[0] == "Note" && operation.OperationId == "Upload")
            {
                operation.Consumes.Add("application/form-data");
            }
        }
    }
}
