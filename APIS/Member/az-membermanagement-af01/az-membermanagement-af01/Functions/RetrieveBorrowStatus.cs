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
            [HttpTrigger(AuthorizationLevel.Function, "Post")] HttpRequest httpRequest,
            ILogger logger)
        {
            //Create a reservation request
            try
            {
                // Read the body 
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                EventSchema reservationEvent = JsonConvert.DeserializeObject<EventSchema>(requestBody);

                
                
                var status = await _sqlHelper.RetrieveBorrowStatus(reservationEvent).ConfigureAwait(false);

                if (status)
                {
                    return new EventGridEvent()
                    {
                        Id = reservationEvent.ID,
                        Data = reservationEvent.BookReservation,
                        EventType = "Accepted",
                        Subject = "BookReservation",
                        DataVersion = "1.0"

                    };
                }
                else {
                    return new EventGridEvent()
                    {
                        Id = reservationEvent.ID,
                        Data = reservationEvent.BookReservation,
                        EventType = "Rejected",
                        Subject = "BookReservation",
                        DataVersion = "1.0"

                    };
                }

            }
            catch (Exception ex)
            {
                return null;
                //Output Exception Event
            }

        }
    }
}
