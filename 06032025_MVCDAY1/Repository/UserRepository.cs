using _06032025_MVCDAY1.Models;
using Humanizer;
using Microsoft.Data.SqlClient;
//using Razorpay.Api;
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
       

        public User getSessionData(string email)
        {
            User user = null;
            using (SqlConnection conn=new SqlConnection(_constring))
            {
                string query = @"SELECT * FROM customer.registeruser WHERE email=@email";
                SqlCommand cmd = new SqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@email",email);
                conn.Open();
                SqlDataReader rd= cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    user = new User
                    {
                        user_id =Convert.ToInt32(rd["user_id"]),
                        email = rd["email"].ToString(),
                        password = rd["password"].ToString(),
                        first_name = rd["first_name"].ToString(),
                        Role_ID = Convert.ToInt32(rd["Role_ID"])
                    };
                }
                conn.Close();
            }
            return user;
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

        //Product side Methods
        public int AddProduct(Product product)
        {
            using (SqlConnection conn=new SqlConnection(_constring))
            {
                string query = @"INSERT INTO vendor.Products(product_name,brand_id,category_id,vendor_id,price,sub_category_id) OUTPUT INSERTED.product_id VALUES (@prodname,@bid,@cid,@vid,@price,@subcatid)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("prodname", product.product_name);
                cmd.Parameters.AddWithValue("bid", product.brand_id);
                cmd.Parameters.AddWithValue("cid", product.category_id);
                cmd.Parameters.AddWithValue("vid", product.vendor_id);
                cmd.Parameters.AddWithValue("price", product.price);
               
                cmd.Parameters.AddWithValue("subcatid", product.sub_category_id);

                conn.Open();
                int product_id = (int)cmd.ExecuteScalar();
                conn.Close();

                return product_id;
            }
        }

        public bool AddProductImage(ProductImage productImage)
        {
           using(SqlConnection conn=new SqlConnection(_constring))
            {
                string query = @"INSERT INTO vendor.prodImages(imgName,imgType,product_id) Values(@imgname,@imgtype,@prodid)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("imgname", productImage.imgName);
                cmd.Parameters.AddWithValue("imgtype", productImage.imgType);
                cmd.Parameters.AddWithValue("prodid", productImage.product_id);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                conn.Close();

                return rows > 0;
            }
        }
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                string query = "SELECT * FROM vendor.Products";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    /*products.Add(new Product
                    {
                        productid = Convert.ToInt32(reader["productid"]),
                        ProductName = reader["ProductName"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        ProductRatings = Convert.ToInt32(reader["ProductRatings"])
                    });*/
                    products.Add(new Product
                    {
                        product_id = reader["product_id"] != DBNull.Value ? Convert.ToInt32(reader["product_id"]) : 0,
                        product_name = reader["product_name"] != DBNull.Value ? reader["product_name"].ToString() : string.Empty,
                        brand_id = reader["brand_id"] != DBNull.Value ? Convert.ToInt32(reader["brand_id"]) : 0,
                        category_id = reader["category_id"] != DBNull.Value ? Convert.ToInt32(reader["category_id"]) : 0,
                        vendor_id = reader["vendor_id"] != DBNull.Value ? Convert.ToInt32(reader["vendor_id"]) : 0,
                        price = reader["price"] != DBNull.Value ? Convert.ToInt32(reader["price"]) : 0,
                        sub_category_id = reader["sub_category_id"] != DBNull.Value ? Convert.ToInt32(reader["sub_category_id"]) : 0,

                    });
                }
                conn.Close();
            }
            return products;
        }

        

        public List<ProductImage> GetImagesByProductId(int productId)
        {
            List<ProductImage> images = new List<ProductImage>();

            using (SqlConnection conn= new SqlConnection(_constring))
            {
                string query = "SELECT * FROM vendor.prodImages WHERE product_id = @productid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@productid", productId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    images.Add(new ProductImage
                    {
                        prod_img_id = Convert.ToInt32(reader["prod_img_id"]),
                        product_id = Convert.ToInt32(reader["product_id"]),
                        imgName = reader["imgName"].ToString(),
                        imgType = reader["imgType"].ToString()
                    });
                }
                conn.Close();
            }
            return images;
        }
        public List<Product> ProductById(int id)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                string query = "SELECT * FROM Supply.Products WHERE productid = @productid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@productid", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        product_id = Convert.ToInt32(reader["product_id"]),
                        product_name = reader["product_name"].ToString(),
                        brand_id = Convert.ToInt32(reader["brand_id"]),
                        category_id = Convert.ToInt32(reader["category_id"]),
                        vendor_id = Convert.ToInt32(reader["vendor_id"]),
                        price = Convert.ToInt32(reader["price"]),
                        sub_category_id = Convert.ToInt32(reader["sub_category_id"]),
                    });
                 
                }
                conn.Close();
            }
            return products;
        }



        //Customer WishList
        public void AddToWishlist(int userId, int productId)
        {
            using (SqlConnection conn=new SqlConnection(_constring))
            {
                string query = @"INSERT INTO customer.Wishlist(user_id,product_id) VALUES (@uid,@prodid)";
                SqlCommand cmd = new SqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@uid",userId);
                cmd.Parameters.AddWithValue("@prodid",productId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void RemoveFromWishlist(int userId, int productId)
        {
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                string query = @"DELETE FROM customer.Wishlist WHERE user_id=@uid AND product_id=@prodid ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@prodid", productId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Product> GetUserWishlist(int userId)
        {
            List<Product> wishlist = new List<Product>();

            using (SqlConnection conn = new SqlConnection(_constring))
            {
                string query = "SELECT p.* FROM vendor.Products p INNER JOIN customer.Wishlist w ON p.product_id = w.product_id WHERE w.user_id = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    wishlist.Add(new Product
                    {
                        product_id = Convert.ToInt32(reader["product_id"]),
                        product_name = reader["product_name"].ToString(),
                        brand_id = Convert.ToInt32(reader["brand_id"]),
                        category_id = Convert.ToInt32(reader["category_id"]),
                        vendor_id = Convert.ToInt32(reader["vendor_id"]),
                        price = Convert.ToInt32(reader["price"]),
                        sub_category_id = Convert.ToInt32(reader["sub_category_id"]),
                    });
                }
            }
            return wishlist;
        }
        public bool IsInWishlist(int productId, int userId)
        {
            using (SqlConnection con = new SqlConnection(_constring))
            {
                string query = "SELECT COUNT(*) FROM customer.Wishlist WHERE product_id = @ProductId AND user_id = @Userid";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Userid", userId);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

    }
}
