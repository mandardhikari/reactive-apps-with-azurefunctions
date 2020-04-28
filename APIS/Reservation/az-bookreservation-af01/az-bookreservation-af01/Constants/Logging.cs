using System;
using System.Collections.Generic;
using System.Text;

namespace az_bookreservation_af01.Constants
{
    internal static class Logging
    {
        //internal const string LoggingTemplate = "{EventDescription} {ProcessingWorkflow} {CorrelationId} {BlobName} {Status} {LogMessage}";
        internal const string LoggingTemplate =
            "{CorrelationID} {Function} {EventType} {Status} {LogMessage}";

        internal const string GenericExceptionLoggingTemplate =
           "{Function} {Status} {LogMessage}";

        internal enum Status
        {
            Started,
            Succeeded,
            Failed,

            //Only Used During Debug Logging
            Processing

        }

        internal enum EventId
        {
            CreateReserveration = 100,
            UpdateReservation = 101
        }
    }
}
