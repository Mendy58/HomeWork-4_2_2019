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
                                     VALUES (@Name, @Description,0,@Password)
                                             Select Scope_Identity();";
                cmd.Parameters.AddWithValue("@Name", picture.Name);
                cmd.Parameters.AddWithValue("@Description", picture.Description);
                cmd.Parameters.AddWithValue("@Password", picture.Password);
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
    }
    //Select Password from Images
    //UPDATE Images  SET Count = Count + 1 WHERE id = 1
}
