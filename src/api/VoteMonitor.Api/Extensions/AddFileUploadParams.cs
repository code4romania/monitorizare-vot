using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VoteMonitor.Api.Extensions
{
    /// <inheritdoc />
    public class AddFileUploadParams : IOperationFilter
    {
        /// <inheritdoc />

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //if (operation.Tags[0].Name == "Observer" && operation.OperationId == "Import" ||
            //    operation.Tags[0].Name == "Note" && operation.OperationId == "Upload") {
            //    operation.RequestBody.Add("application/form-data");
            //}
        }
    }
}
