using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using az_bookmanagement_af01.Interfaces;
using System.Threading.Tasks;
using az_bookmanagement_af01.Models;
using Microsoft.Extensions.Configuration;

namespace az_bookmanagement_af01.Helpers
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


        public async Task LockBook(EventSchema reservationEvent)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("Book.uspLockBook", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ISBN", reservationEvent.BookReservation.ISBN);
                    command.Parameters["@ISBN"].Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.Add("@Lock", System.Data.SqlDbType.Int);

                    switch (reservationEvent.EventType)
                    {
                        case "Requested":
                            command.Parameters["@Lock"].Value = 1;
                            break;
                        case "Rejected":
                            command.Parameters["@Lock"].Value = 0;
                            break;

                        case "Accepted":
                            command.Parameters["@Lock"].Value = 2;
                            break;
                    }   
                    
                    command.Parameters["@Lock"].Direction = System.Data.ParameterDirection.Input;


                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                }

            }

        }
    }
}
