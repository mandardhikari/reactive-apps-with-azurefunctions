using az_bookmanagement_af01.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace az_bookmanagement_af01.Interfaces
{
    public interface ISqlHelper
    {
        Task LockBook(EventSchema bookReservation);
        
    }
}
