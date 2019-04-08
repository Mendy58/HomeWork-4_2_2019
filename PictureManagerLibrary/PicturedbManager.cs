using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureManagerLibrary
{
    public class PicturedbManager
    {
        private string _connectionstring;
        public PicturedbManager(string _ConnectionString)
        {
            _connectionstring = _ConnectionString;
        }

        public List<string> GetPasswords()
        {
            var p = new List<string>();
            using (var Con = new SqlConnection(_connectionstring))
            using (var cmd = Con.CreateCommand())
            {
                cmd.CommandText = @"Select Password from Images";
                Con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    p.Add((string)reader["Password"]);
                }
            }
            return p;
        }

        public List<Picture> GetPictureByUserid(int Userid)
        {
            List<Picture> p = new List<Picture>();
            using (var Con = new SqlConnection(_connectionstring))
            using (var cmd = Con.CreateCommand())
            {
                cmd.CommandText = @"Select * from Images
                                  Where Personid = @id";
                cmd.Parameters.AddWithValue("@id", Userid);
                Con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                while(reader.Read())
                {
                    p.Add(new Picture
                    {
                        id = (int)reader["id"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"],
                        Password = (string)reader["Password"],
                        PersonId = (int)reader["PersonId"],
                        Count = (int)reader["Count"],
                    });
                }
            }
            return p;
        }

        public Picture GetPictureByid(int id)
        {
            Picture p = new Picture();
            using (var Con = new SqlConnection(_connectionstring))
            using (var cmd = Con.CreateCommand())
            {
                cmd.CommandText = @"Select * from Images
                                  Where id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                Con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                p.id = (int)reader["id"];
                p.Name = (string)reader["Name"];
                p.Description = (string)reader["Description"];
                p.Password = (string)reader["Password"];
                p.PersonId = (int)reader["PersonId"];
                p.Count = (int)reader["Count"];
            }
            return p;
        }
        public int AddPic(Picture picture)
        {
            int x;
            using (var con = new SqlConnection(_connectionstring))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Images
                                     VALUES (@Name, @Description,0,@Userid,@Password)
                                             Select Scope_Identity();";
                cmd.Parameters.AddWithValue("@Name", picture.Name);
                cmd.Parameters.AddWithValue("@Description", picture.Description);
                cmd.Parameters.AddWithValue("@Password", picture.Password);
                cmd.Parameters.AddWithValue("@Userid", picture.PersonId);
                con.Open();
                x = (int) (decimal) cmd.ExecuteScalar();
            }
            return x;
        }
        public void AddCountToPic(int id)
        {
            using (var con = new SqlConnection(_connectionstring))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Images  SET Count = Count + 1 WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public User GetByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                return new User
                {
                    id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    PasswordHash = (string)reader["PasswordHash"],
                    Email = (string)reader["Email"]
                };
            }
        }
        public void AddUser(User user, string password)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = hash;
            using (var connection = new SqlConnection(_connectionstring))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                                  "VALUES (@name, @email, @hash)";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@hash", user.PasswordHash);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public User Login(String email, string password)
        {
            User s = GetByEmail(email);
            if(s==null || !BCrypt.Net.BCrypt.Verify(password, s.PasswordHash))
            {
                return null;
            }
            return s;
        }
    }
    //Select Password from Images
    //UPDATE Images  SET Count = Count + 1 WHERE id = 1
}
