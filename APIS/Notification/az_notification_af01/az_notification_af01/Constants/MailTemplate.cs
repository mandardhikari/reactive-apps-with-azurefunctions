using System;
using System.Collections.Generic;
using System.Text;

namespace az_notification_af01.Constants
{
    public class MailTemplate
    {
        public const string Subject = "Book reservation alert";

        public const string AcceptedBody = @"<p>Dear {0},</p>
<p>This is a notification email that you have succesfully reserved following book</p>
<table>
<tbody>
<tr>
<td>SrNo</td>
<td>Name</td>
<td>Author</td>
<td>ISBN</td>
</tr>
<tr>
<td>{1}</td>
<td>{2}</td>
<td>{3}</td>
<td>{4}</td>
</tr>
</tbody>
</table>
<p>You can now claim the book within next 3 business day, if unclaimed, your reservation stands to cancel. Please quote {5} as your request id for any support related cases.</p>
<p>Regards,</p>
<p>Libaray Management Team</p>";

        public const string RejectedBody = @"<p>Dear {0},</p>
<p>We regret to inform you that following book</p>
<table>
<tbody>
<tr>
<td>SrNo</td>
<td>Name</td>
<td>Author</td>
<td>ISBN</td>
</tr>
<tr>
<td>{1}</td>
<td>{2}</td>
<td>{3}</td>
<td>{4}</td>
</tr>
</tbody>
</table>
<p>can not be reserved as you already have a different book reserved.</p>
<p>Please return the book and try reserving again.</p>
<p>Regards,</p>
<p>Libaray Management Team</p>";

        public const string From = "noreply@demolibrarymanagement.com";
    }
}
