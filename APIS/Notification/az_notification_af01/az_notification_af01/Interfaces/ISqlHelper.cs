using az_notification_af01.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace az_notification_af01.Interfaces
{
    public interface ISqlHelper
    {
        Task<Member> RetrieveMember(EventSchema bookResgistration);
        
        
    }
}
