using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VoteMonitor.Api.Core
{
    public static class ControllerExtensions
    {
        public static readonly string ID_NGO_VALUE = "IdNgo";
        public static readonly string ORGANIZER_VALUE = "Organizer";
        public static readonly string TOKEN_VALUE = "Token";
        public static readonly string AUTH_HEADER_VALUE = "Authorization";
        public static readonly string BEARER_VALUE = "Bearer ";
        public static readonly int LOWER_OBS_VALUE = 1;
        public static readonly int UPPER_OBS_VALUE = 300;
        public static readonly string RESET_ERROR_MESSAGE = "Internal server error, please verify that provided id is correct ";
        public static readonly string DEVICE_RESET = "device";
        public static readonly string PASSWORD_RESET = "password";

        public static int GetIdOngOrDefault(this Controller controller, int defaultIdOng)
        {
            int result;
            return int.TryParse(controller.User.Claims.FirstOrDefault(a => a.Type == ID_NGO_VALUE)?.Value, out result)
                ? result
                : defaultIdOng;
        }

        public static bool GetOrganizatorOrDefault(this Controller controller, bool defaultOrganizator)
        {
            bool result;
            return bool.TryParse(controller.User.Claims.FirstOrDefault(a => a.Type == ORGANIZER_VALUE)?.Value, out result)
                ? result
                : defaultOrganizator;
        }

        public static bool ValidateGenerateObserversNumber(int number)
        {
            return ((number > LOWER_OBS_VALUE) && (number < UPPER_OBS_VALUE));
        }
        public static IAsyncResult ResultAsync(this Controller controller, HttpStatusCode statusCode, ModelStateDictionary modelState = null)
        {
            controller.Response.StatusCode = (int)statusCode;

            if (modelState == null)
                return Task.FromResult(new StatusCodeResult((int)statusCode));

            return Task.FromResult(controller.BadRequest(modelState));
        }
    }
}
