using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace az_membermanagement_af01.Models
{
    public class BookReservation
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "isbn")]
        public string ISBN { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "memberId")]
        public int MemberID { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "correlationId")]
        public string CorrelationID { get; set; }

    }
}
