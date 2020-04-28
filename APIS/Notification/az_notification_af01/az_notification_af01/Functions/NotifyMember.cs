using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using az_notification_af01.Interfaces;
using az_notification_af01.Constants;
using az_notification_af01.Models;
using SendGrid.Helpers.Mail;

namespace az_notification_af01.Functions
{
    public class NotifyMember
    {
        private readonly ISqlHelper _sqlHelper;

        public NotifyMember(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));

        }

        [FunctionName(nameof(NotifyMember))]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "Post")] HttpRequest httpRequest,
            [SendGrid(ApiKey = "AzureWebJobsSendGridApiKey")]  IAsyncCollector<SendGridMessage> asyncCollector,
            ILogger logger)
        {
            //Create a reservation request
            try
            {
                // Read the body 
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                EventSchema reservationEvent = JsonConvert.DeserializeObject<EventSchema>(requestBody);

                logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.NotifyMember)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(NotifyMember),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Started.ToString(),
                      "Notifying Member about status of book reservation."
                      );



                var member = await _sqlHelper.RetrieveMember(reservationEvent);

                var emailMessage = new SendGridMessage();
                emailMessage.AddTo(member.Email);
                emailMessage.SetFrom(MailTemplate.From);
                emailMessage.SetSubject(MailTemplate.Subject);
                string emailBody = string.Empty;

                switch (reservationEvent.EventType)
                {
                    case ReservationStatus.Created:
                        emailBody = string.Format(MailTemplate.AcceptedBody, member.Name, "1",
                        reservationEvent.BookReservation.Name,
                        reservationEvent.BookReservation.Author,
                        reservationEvent.BookReservation.ISBN,
                        reservationEvent.BookReservation.CorrelationID);
                        break;
                    case ReservationStatus.Rejected:
                        emailBody = string.Format(MailTemplate.RejectedBody, member.Name, "1",
                        reservationEvent.BookReservation.Name,
                        reservationEvent.BookReservation.Author,
                        reservationEvent.BookReservation.ISBN
                        );
                        break;
                }




                emailMessage.AddContent("text/html", emailBody);

                await asyncCollector.AddAsync(emailMessage);

                logger.LogInformation(new EventId(Convert.ToInt32(Logging.EventId.NotifyMember)),
                      Logging.LoggingTemplate,
                      reservationEvent.BookReservation.CorrelationID,
                      nameof(NotifyMember),
                      reservationEvent.EventType.ToString(),
                      Logging.Status.Succeeded.ToString(),
                      "Handing over notification to Send Grid server."
                      );

            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(Convert.ToInt32(Logging.EventId.NotifyMember)),
                      Logging.GenericExceptionLoggingTemplate,
                      nameof(NotifyMember),
                      Logging.Status.Failed.ToString(),
                      string.Format("Failed while notifying Member of state of book reservation. Exception {0}", ex.Message)
                      );
                //Output Exception Event
            }

        }
    }
}
