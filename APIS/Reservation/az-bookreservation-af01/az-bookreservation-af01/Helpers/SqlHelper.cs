using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using az_bookreservation_af01.Interfaces;
using System.Threading.Tasks;
using az_bookreservation_af01.Models;
using Microsoft.Extensions.Configuration;

namespace az_bookreservation_af01.Helpers
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

        public async Task CreateReservation(EventSchema reservationEvent)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("Reservation.uspCreateReservation", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CorrelationID", reservationEvent.BookReservation.CorrelationID);
                    command.Parameters["@CorrelationID"].Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.AddWithValue("@MemberID", reservationEvent.BookReservation.MemberID);
                    command.Parameters["@MemberID"].Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.AddWithValue("@ISBN", reservationEvent.BookReservation.ISBN);
                    command.Parameters["@ISBN"].Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.AddWithValue("@Status", reservationEvent.EventType.ToString());
                    command.Parameters["@Status"].Direction = System.Data.ParameterDirection.Input;

                    await connection.OpenAsync().ConfigureAwait(false);

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    await connection.CloseAsync().ConfigureAwait(false);

                }

            }


        }

        public async Task UpdateReservation(EventSchema reservationEvent)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("Reservation.uspUpdateReservation", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CorrelationID", reservationEvent.BookReservation.CorrelationID);
                    command.Parameters["@CorrelationID"].Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.AddWithValue("@Status", reservationEvent.EventType.ToString());
                    command.Parameters["@Status"].Direction = System.Data.ParameterDirection.Input;

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                }

            }

        }
    }
}
