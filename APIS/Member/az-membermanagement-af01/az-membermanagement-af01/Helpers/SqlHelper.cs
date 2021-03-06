﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using az_membermanagement_af01.Interfaces;
using System.Threading.Tasks;
using az_membermanagement_af01.Models;
using Microsoft.Extensions.Configuration;

namespace az_membermanagement_af01.Helpers
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


        public async Task<int> UpdateBorrowedBook(EventSchema reservationEvent)
        {
            int numberofRowsUpdated = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("Member.uspUpdateBorrowedBook", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@MemberID", reservationEvent.BookReservation.MemberID);
                    command.Parameters["@MemberID"].Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.Add("@ISBN", System.Data.SqlDbType.NVarChar, 20).Direction =
                        System.Data.ParameterDirection.Input;

                    command.Parameters.Add("@CanBorrow", System.Data.SqlDbType.Bit).Direction =
                        System.Data.ParameterDirection.Input;

                    switch(reservationEvent.EventType)
                    {
                        case ReservationStatus.Accepted:
                            command.Parameters["@ISBN"].Value = reservationEvent.BookReservation.ISBN;
                            command.Parameters["@CanBorrow"].Value = false;
                            break;

                        case ReservationStatus.Exceptioned:
                            command.Parameters["@ISBN"].Value = DBNull.Value;
                            command.Parameters["@CanBorrow"].Value = true;
                            break;

                    }
                    await connection.OpenAsync().ConfigureAwait(false);

                    numberofRowsUpdated = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                }

            }
            return numberofRowsUpdated;
        }

        public async Task<bool> RetrieveBorrowStatus(EventSchema reservationEvent)
        {
            bool retVal = false;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("Member.uspRetrieveBorrowStatus", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@MemberID", reservationEvent.BookReservation.MemberID);
                    command.Parameters["@MemberID"].Direction = System.Data.ParameterDirection.Input;
                    command.Parameters.Add("@CanBorrow", System.Data.SqlDbType.Bit);
                    command.Parameters["@CanBorrow"].Direction = System.Data.ParameterDirection.Output;

                    await connection.OpenAsync().ConfigureAwait(false);

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    retVal = Convert.ToBoolean(command.Parameters["@CanBorrow"].Value);

                }

            }

            return retVal;
        }
    }
}
