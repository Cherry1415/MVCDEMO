using _06032025_MVCDAY1.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _06032025_MVCDAY1.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly string _constring; 

        public UserRepository(IConfiguration configuration)
        {
            _constring = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Product> GetAllProduct()
        {
            List<Product> prod = new List<Product>();

            using (SqlConnection con = new SqlConnection(_constring))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT ", con);
               // cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                //while (reader.Read())
                //{
                //    prod.Add(
                //        new Product
                //        {
                //            EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                //            Name = reader["Name"].ToString(),
                //            Password = reader["Password"].ToString(),
                //            Email = reader["Email"].ToString(),
                //            PhoneNumber = reader["PhoneNumber"].ToString(),
                //            RollID = Convert.ToInt32(reader["RollID"]),
                //            DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                //            Salary = Convert.ToDecimal(reader["Salary"]),
                //            Hobbies = reader["Hobbies"] != DBNull.Value ? reader["Hobbies"].ToString().Split(',').ToList() : new List<string>(),
                //            profileimg = reader["profileimg"] as byte[]

                //        });
                //}
            }
            return prod;
        }

        public bool Login(string email, string password)
        {
            using (SqlConnection conn=new SqlConnection(_constring))
            {
                string query = @"SELECT COUNT(*) FROM customer.registeruser WHERE email=@email and password=@password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);
                conn.Open();
                int cnt = (int)cmd.ExecuteScalar();
                conn.Close();
                return cnt > 0;
            }
        }

        public bool Register(User user)
        {
            using (SqlConnection conn=new SqlConnection(_constring))
            {
                SqlCommand  cmd= new SqlCommand("customer.sp_registeruser", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fname", user.first_name);
                cmd.Parameters.AddWithValue("@lname", user.last_name);
                cmd.Parameters.AddWithValue("@email",user.email);
                cmd.Parameters.AddWithValue("@phone",user.phone);
                cmd.Parameters.AddWithValue("@pass",user.password);
                cmd.Parameters.AddWithValue("@roleid",user.Role_ID);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();
                return rowsAffected > 0;
            }
        }
    }
}
