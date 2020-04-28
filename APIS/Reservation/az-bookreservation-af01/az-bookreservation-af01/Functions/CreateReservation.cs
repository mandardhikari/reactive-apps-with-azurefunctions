using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using az_bookreservation_af01.Interfaces;
using az_bookreservation_af01.Models;
using az_bookreservation_af01.Constants;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json.Linq;

namespace az_bookreservation_af01.Functions
{

    public class CreateReservation
    {
        private readonly ISqlHelper _sqlHelper;

        public CreateReservation(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));

        }

        [FunctionName(nameof(CreateReservation))]
        public async Task Run(
            [EventGridTrigger()] EventGridEvent eventGridEvent,
            ILogger logger)
        {
            try
            {

                EventSchema reservationEvent = new EventSchema()
                {
                    ID = eventGridEvent.Id,
                    BookReservation = ((JObject)eventGridEvent.Data).ToObject<BookReservation>(),
                    EventTime = eventGridEvent.EventTime,
                    EventType = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), eventGridEvent.EventType),
                    Subject = eventGridEvent.Subject

                };
                
                

                try
                {
                    logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.CreateReserveration)),
                        Logging.LoggingTemplate,
                        reservationEvent.BookReservation.CorrelationID,
                        nameof(CreateReservation),
                        reservationEvent.EventType.ToString(),
                        Logging.Status.Started.ToString(),
                        "Creating auditing entry."
                        );

                    await _sqlHelper.CreateReservation(reservationEvent).ConfigureAwait(false);

                    logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.CreateReserveration)),
                        Logging.LoggingTemplate,
                        reservationEvent.BookReservation.CorrelationID,
                        nameof(CreateReservation),
                        reservationEvent.EventType.ToString(),
                        Logging.Status.Succeeded.ToString(),
                        "Completed creating auditing entry."
                        );

                }
                catch (Exception ex)
                {
                    //Output Exception Event
                    logger.LogError(new EventId(Convert.ToInt32(Logging.EventId.CreateReserveration)),
                       Logging.LoggingTemplate,
                       reservationEvent.BookReservation.CorrelationID,
                       nameof(CreateReservation),
                       reservationEvent.EventType.ToString(),
                       Logging.Status.Failed.ToString(),
                       string.Format("Failed while creating auditing entry. Exception {0}", ex.Message)
                       );
                }
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(Convert.ToInt32(Logging.EventId.CreateReserveration)),
                        Logging.GenericExceptionLoggingTemplate,
                        nameof(CreateReservation),
                        Logging.Status.Failed.ToString(),
                        string.Format("Failed while parsing incoming event. Exception {0}", ex.Message)
                        );
            }
            
            

        }

    }
}
