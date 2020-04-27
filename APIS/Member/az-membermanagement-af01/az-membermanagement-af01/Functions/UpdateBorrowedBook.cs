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

               var retVal =  await _sqlHelper.UpdateBorrowedBook(reservationEvent).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                //Output Exception Event
            }

        }
    }
}
