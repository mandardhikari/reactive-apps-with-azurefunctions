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
            [HttpTrigger(AuthorizationLevel.Function, "Post")] HttpRequest httpRequest,
            ILogger logger)
        {
            try
            {
                // Read the body 
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                EventSchema reservationEvent = JsonConvert.DeserializeObject<EventSchema>(requestBody);

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
            //Create a reservation request
            

        }

    }


//    public static class CreateBookReservation
//    {
//        [FunctionName("CreateBookReservation")]
//        public static async Task<IActionResult> Run(
//            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
//            ILogger log)
//        {
//            log.LogInformation("C# HTTP trigger function processed a request.");

//            string name = req.Query["name"];

//            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
//            dynamic data = JsonConvert.DeserializeObject(requestBody);
//            name = name ?? data?.name;

//            string responseMessage = string.IsNullOrEmpty(name)
//                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
//                : $"Hello, {name}. This HTTP triggered function executed successfully.";

//            return new OkObjectResult(responseMessage);
//        }
//    }
}
