using System;
using VoteMonitor.Api.Location.Exceptions;

namespace VoteMonitor.Api.Location.Models.ResultValues
{
    public class PollingStationImportResultValue
    {
        public bool Success { get { return Error == null; } }
        public PollingStationImportErrorModel Error { get; }

        public static PollingStationImportResultValue SuccessValue = new PollingStationImportResultValue();
        private PollingStationImportResultValue()
        {

        }

        public PollingStationImportResultValue(Exception exception)
        {
            PollingStationImportErrorCode errorCode;

            if (exception.GetType() == typeof(PollingStationImportException))
            {
                errorCode = PollingStationImportErrorCode.CountyNotFound;
            }
            else
            {
                errorCode = PollingStationImportErrorCode.GenericError;
            }

            Error = new PollingStationImportErrorModel(errorCode, exception);
        }
    }

    public class PollingStationImportErrorModel
    {
        public PollingStationImportErrorCode ErrorCode { get; }
        public Exception Exception { get; }

        public PollingStationImportErrorModel(PollingStationImportErrorCode errorCode, Exception exception)
        {
            ErrorCode = errorCode;
            Exception = exception;
        }
    }
}
