using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using az_bookmanagement_af01.Interfaces;
using az_bookmanagement_af01.Models;
using az_bookmanagement_af01.Constants;

namespace az_bookreservation_af01.Functions
{
    public class ReserveBook
    {
        private readonly ISqlHelper _sqlHelper;

        public ReserveBook(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));

        }

        [FunctionName(nameof(ReserveBook))]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "Post")] HttpRequest httpRequest,
            ILogger logger)
        {
            // Read the body 
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                EventSchema reservationEvent = JsonConvert.DeserializeObject<EventSchema>(requestBody);

                //Create a reservation request
                try
                {
                    logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.ReserveBook)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(ReserveBook),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Started.ToString(),
                      "Updating Book status based on event"
                      );

                    await _sqlHelper.LockBook(reservationEvent).ConfigureAwait(false);

                    logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.ReserveBook)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(ReserveBook),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Succeeded.ToString(),
                      "Updated Book status based on event"
                      );

                }
                catch (Exception ex)
                {
                    logger.LogError(new EventId(Convert.ToInt32(Logging.EventId.ReserveBook)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(ReserveBook),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Failed.ToString(),
                      string.Format("Failed updating Book status based on event.Exception {0}", ex.Message)
                      );
                    //Output Exception Event
                }

            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(Convert.ToInt32(Logging.EventId.ReserveBook)),
                                        Logging.GenericExceptionLoggingTemplate,
                                        nameof(ReserveBook),
                                        Logging.Status.Failed.ToString(),
                                        string.Format("Failed while parsing incoming event. Exception {0}", ex.Message)
                                        );
            }
        }
    }
}
