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
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using az_membermanagement_af01.Constants;
using Newtonsoft.Json.Linq;

namespace az_membermanagement_af01.Functions
{
    public class RetrieveBorrowStatus
    {

        private readonly ISqlHelper _sqlHelper;

        public RetrieveBorrowStatus(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));

        }

        [FunctionName(nameof(RetrieveBorrowStatus))]
        [return: EventGrid(TopicEndpointUri = "topicEndpointUri", 
            TopicKeySetting = "topicKey")]
        public async Task<EventGridEvent> Run(
            [EventGridTrigger()] EventGridEvent eventGridEvent,
            ILogger logger)
        {
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
                // Read the body 

                logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.RetrieveBorrowStatus)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(RetrieveBorrowStatus),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Started.ToString(),
                      "Retrieveing borrow status of member."
                      );


                var status = await _sqlHelper.RetrieveBorrowStatus(reservationEvent).ConfigureAwait(false);

                logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.RetrieveBorrowStatus)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(RetrieveBorrowStatus),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Succeeded.ToString(),
                      "Retrieved borrow status of member. Generating Event"
                      );

                if (status)
                {
                    return new EventGridEvent()
                    {
                        Id = reservationEvent.ID,
                        Data = reservationEvent.BookReservation,
                        EventType = ReservationStatus.Accepted.ToString(),
                        Subject = "BookReservation",
                        DataVersion = "1.0"

                    };
                }
                else {
                    return new EventGridEvent()
                    {
                        Id = reservationEvent.ID,
                        Data = reservationEvent.BookReservation,
                        EventType = ReservationStatus.Rejected.ToString(),
                        Subject = "BookReservation",
                        DataVersion = "1.0"

                    };
                }

            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(Convert.ToInt32(Logging.EventId.RetrieveBorrowStatus)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(RetrieveBorrowStatus),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Failed.ToString(),
                      string.Format("Error retrieveing borrow status of member. Exception {0}", ex.Message)
                      ) ;

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
