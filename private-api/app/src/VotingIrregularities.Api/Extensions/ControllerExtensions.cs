using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.Swagger.Model;

namespace VotingIrregularities.Api.Extensions
{
    public static class ControllerExtensions
    {

        public static IAsyncResult ResultAsync(this Controller controller, HttpStatusCode statusCode, ModelStateDictionary modelState = null)
        {
            controller.Response.StatusCode = (int)statusCode;

            if (modelState == null)
                return Task.FromResult(new StatusCodeResult((int)statusCode));

            return Task.FromResult(controller.BadRequest(modelState));
        }
    }
}
