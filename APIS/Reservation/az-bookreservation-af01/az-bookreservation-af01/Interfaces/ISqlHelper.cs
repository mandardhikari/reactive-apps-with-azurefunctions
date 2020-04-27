using az_bookreservation_af01.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace az_bookreservation_af01.Interfaces
{
    public interface ISqlHelper
    {
        Task CreateReservation(EventSchema bookReservation);

        Task UpdateReservation(EventSchema bookReservation);
        
    }
}
