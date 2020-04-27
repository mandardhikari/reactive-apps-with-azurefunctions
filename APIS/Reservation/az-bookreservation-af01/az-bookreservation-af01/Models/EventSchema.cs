using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace az_bookreservation_af01.Models
{
    public class EventSchema
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "eventTime")]
        public DateTime EventTime { get; set; }

        [JsonProperty(PropertyName = "eventType")]
        public string EventType { get; set; }

        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "bookReservation")]
        public BookReservation BookReservation { get; set; }

        [JsonProperty(PropertyName = "memberId")]
        public int MemberID { get; set; }


    }
}
