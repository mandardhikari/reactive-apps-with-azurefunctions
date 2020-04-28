using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using az_membermanagement_af01.Interfaces;
using az_membermanagement_af01.Models;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using az_membermanagement_af01.Constants;
using Newtonsoft.Json.Linq;

namespace az_membermanagement_af01.Functions
{
    public class UpdateBorrowedBook
    {
        private readonly ISqlHelper _sqlHelper;

        public UpdateBorrowedBook(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));

        }

        [FunctionName(nameof(UpdateBorrowedBook))]
        [return: EventGrid(TopicEndpointUri = "topicEndpointUri",
            TopicKeySetting = "topicKey")]
        public async Task<EventGridEvent> Run(
            [EventGridTrigger()] EventGridEvent eventGridEvent,
            ILogger logger)
        {
            // Read the body 
            EventSchema reservationEvent = new EventSchema()
            {
                ID = eventGridEvent.Id,
                BookReservation = ((JObject)eventGridEvent.Data).ToObject<BookReservation>(),
                EventTime = eventGridEvent.EventTime,
                EventType = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), eventGridEvent.EventType),
                Subject = eventGridEvent.Subject

            };

            //Create a reservation request
            try
            {
                

                logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.UpdateBorrowedBook)),
                     Logging.LoggingTemplate,
                     reservationEvent.BookReservation.CorrelationID,
                     nameof(UpdateBorrowedBook),
                     reservationEvent.EventType.ToString(),
                     Logging.Status.Started.ToString(),
                     "Updating borrowed book by the member."
                     );


                var retVal = await _sqlHelper.UpdateBorrowedBook(reservationEvent).ConfigureAwait(false);

              

                if (retVal == 1)
                {
                    logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.UpdateBorrowedBook)),
                  Logging.LoggingTemplate,
                  reservationEvent.BookReservation.CorrelationID,
                  nameof(UpdateBorrowedBook),
                  reservationEvent.EventType.ToString(),
                  Logging.Status.Succeeded.ToString(),
                  "Updated borrowed book by the member."
                  );
                    return new EventGridEvent()
                    {
                        Id = reservationEvent.ID,
                        Data = reservationEvent.BookReservation,
                        EventType = ReservationStatus.Created.ToString(),
                        Subject = "BookReservation",
                        DataVersion = "1.0"

                    };

                }
                else
                {
                    logger.LogError(new EventId(Convert.ToInt32(Logging.EventId.UpdateBorrowedBook)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(UpdateBorrowedBook),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Failed.ToString(),
                      string.Format("Failed to update borrowed book by the member.")
                      );

                    return new EventGridEvent()
                    {
                        Id = reservationEvent.ID,
                        Data = reservationEvent.BookReservation,
                        EventType = ReservationStatus.Exceptioned.ToString(),
                        Subject = "BookReservation",
                        DataVersion = "1.0"

                    };
                }

                

            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(Convert.ToInt32(Logging.EventId.UpdateBorrowedBook)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(UpdateBorrowedBook),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Failed.ToString(),
                      string.Format("Failed to update borrowed book by the member. Error {0}", ex.Message)
                      );

                //Output Exception Event
                return new EventGridEvent()
                {
                    Id = reservationEvent.ID,
                    Data = reservationEvent.BookReservation,
                    EventType = ReservationStatus.Exceptioned.ToString(),
                    Subject = "BookReservation",
                    DataVersion = "1.0"

                };
            }

        }
    }
}
