using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using az_notification_af01.Interfaces;
using System.Threading.Tasks;
using az_notification_af01.Models;
using Microsoft.Extensions.Configuration;

namespace az_notification_af01.Helpers
{
    public class SqlHelper : ISqlHelper
    {
        private readonly IConfiguration _configuration;

        private readonly string _connectionString;

        public SqlHelper(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _connectionString = _configuration.GetConnectionString("LibraryDb");

        }

        public async Task<Member> RetrieveMember(EventSchema reservationEvent)
        {
            Member member;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("Member.uspRetrieveMemberDetails", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@MemberID", reservationEvent.BookReservation.MemberID);
                    command.Parameters["@MemberID"].Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, -1).Direction =
                        System.Data.ParameterDirection.Output;

                    command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, -1).Direction =
                        System.Data.ParameterDirection.Output;

                    await connection.OpenAsync().ConfigureAwait(false);

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    member = new Member()
                    {
                        Name = Convert.ToString(command.Parameters["@Name"].Value),
                        Email = Convert.ToString(command.Parameters["@Email"].Value)
                    };

                }

            }


            return member;
        }
    }
}
