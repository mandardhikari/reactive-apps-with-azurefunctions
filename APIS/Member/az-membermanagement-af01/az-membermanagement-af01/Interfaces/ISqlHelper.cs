using az_membermanagement_af01.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace az_membermanagement_af01.Interfaces
{
    public interface ISqlHelper
    {
        Task<int> UpdateBorrowedBook(EventSchema bookReservation);

        Task<bool> RetrieveBorrowStatus(EventSchema bookReservation);
        
    }
}
