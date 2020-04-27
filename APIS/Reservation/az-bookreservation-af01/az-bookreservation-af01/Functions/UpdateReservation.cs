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

namespace az_bookreservation_af01.Functions
{
    public class UpdateReservation
    {
        private readonly ISqlHelper _sqlHelper;

        public UpdateReservation(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));

        }

        [FunctionName(nameof(UpdateReservation))]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "Post")] HttpRequest httpRequest,
            ILogger logger)
        {
            //Create a reservation request
            try
            {
                // Read the body 
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                EventSchema reservationEvent = JsonConvert.DeserializeObject<EventSchema>(requestBody);

                await _sqlHelper.UpdateReservation(reservationEvent).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                //Output Exception Event
            }

        }
    }
}
