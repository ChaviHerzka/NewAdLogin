using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace NewAddLogin.Data
{
    public class AdDb
    {
        private readonly string _connectionString;
        public AdDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Ad> GetAllAds()
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"select *
                                   from Ads a
                                   join Users u
                                   on a.UserId = u.id 
                                   order by a.Date desc, a.id desc";
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Ad> ads = new List<Ad>();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    UserId = (int)reader["UserId"],
                    Description = (string)reader["Description"],
                    PhoneNumber = (string)reader["PhoneNumber"]
                });
            }
            return ads;
        }
        public void AddUser(User user, string password)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"Insert into Users values(@name, @email, @passwordhash)";
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            command.Parameters.AddWithValue("@passwordhash", user.PasswordHash);
            connection.Open();
            command.ExecuteNonQuery();

        }
        public void AddAd(Ad ad, int userId)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"Insert into Ads (PhoneNumber, Date, Description, UserId)
                                    values( @phoneNumber, getdate(), @description, @userId)";
            command.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            command.Parameters.AddWithValue("@description", ad.Description);
            command.Parameters.AddWithValue("@userId", userId);
            connection.Open();
            command.ExecuteNonQuery();

        }
        public User GetByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"Select * from Users where Email = @email";
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Email = (string)reader["Email"],
                        PasswordHash = (string)reader["PasswordHash"],
                    };
                }
                else
                {
                    return null;
                }
            }
        }
        public User Login(string email, string password)
        {
            User user = GetByEmail(email); ;
            if (user == null)
            {
                return null;
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;

        }
        public void DeleteAdd(int adId)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"Delete from Ads where Id = @adId";
            command.Parameters.AddWithValue("@adId", adId);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public List<Ad> MyAccount(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"select *, a.Id as AdId
                                   from users u 
                                   join ads a 
                                   on a.UserId = u.Id
                                   where u.Id = @id
                                   order by a.date desc";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Ad> ads = new List<Ad>();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["AdId"],
                    UserId = (int)reader["UserId"],
                    Description = (string)reader["Description"],
                    PhoneNumber = (string)reader["PhoneNumber"]

                });
                
            }
            return ads;

        }
    }
}
