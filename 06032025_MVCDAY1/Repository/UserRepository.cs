using _06032025_MVCDAY1.Models;
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
                        sub_category_id = Convert.ToInt32(reader["subcategory_id"]),
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

        //customer Cart
        public void AddToCart(int productId, int quantity, decimal price, int userId)
        {
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand("InsertOrUpdateCart", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@Price", price); // Optional if price is needed

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddItemToCart(int userId, int productId, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand("InsertOrUpdateCart", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void RemoveCartItem(int cartId)
        {
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                string query = "DELETE FROM customer.Cart_Items WHERE cart_item_id = @CartId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CartId", cartId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetCartItemCount(int userId)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(Quantity), 0) FROM customer.Cart_Items WHERE user_id = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    count = Convert.ToInt32(result);
                }
            }
            return count;
        }

        public List<CartItemViewModel> GetCartItemsByUserId(int userId)
        {
            var items = new List<CartItemViewModel>();

            using (SqlConnection conn = new SqlConnection(_constring))
            {
                /* string query = @"SELECT c.cartId AS CartId, p.productid, p.ProductName, p.Price, pi.ImagePath, c.Quantity
                         FROM crm.cart c
                         JOIN supply.Products p ON c.ProductId = p.productid
                         INNER JOIN supply.ProductImage pi
                         ON p.productid=pi.productid
                         WHERE c.customer_id = @UserId";*/
                string query = @"
          SELECT
          c.cart_item_id AS CartId,
          p.product_id,
          p.product_name,
          p.price,
          pi.imgName,
          c.quantity
      FROM customer.Cart_Items c
      INNER JOIN vendor.Products p ON c.product_id = p.product_id
      OUTER APPLY (
          SELECT TOP 1 imgName
          FROM vendor.prodImages
          WHERE product_id = p.product_id
      ) pi
      WHERE c.user_id = @userId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new CartItemViewModel
                            {
                                cart_item_id = Convert.ToInt32(reader["CartId"]),
                                product_id = Convert.ToInt32(reader["product_id"]),
                                product_name = reader["product_name"].ToString(),
                                price = Convert.ToDecimal(reader["price"]),
                                imgName = reader["imgName"].ToString(),
                                quantity = Convert.ToInt32(reader["quantity"])
                            });
                        }
                    }
                }
            }

            return items;
        }
        public CartItemViewModel GetCartItemById(int cartId, int userId)
        {
            CartItemViewModel item = null;

            using (SqlConnection conn = new SqlConnection(_constring))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT c.cart_item_id, c.product_id, c.quantity, p.product_name, p.price, pi.imgName " +
                                                "FROM customer.Cart_Items c " +
                                                "INNER JOIN vendor.Products p ON c.product_id = p.product_id " +
                                                "LEFT JOIN vendor.prodImages pi ON pi.product_id = p.product_id " +
                                                "WHERE c.cart_item_id = @CartId AND c.user_id = @CustomerId", conn);
                cmd.Parameters.AddWithValue("@CartId", cartId);
                cmd.Parameters.AddWithValue("@CustomerId", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new CartItemViewModel
                        {
                            cart_item_id = Convert.ToInt32(reader["cart_item_id"]),
                            product_id = Convert.ToInt32(reader["product_id"]),
                            product_name = reader["product_name"].ToString(),
                            price = Convert.ToDecimal(reader["price"]),
                            quantity = Convert.ToInt32(reader["quantity"]),
                            imgName = reader["imgName"] != DBNull.Value ? reader["imgName"].ToString() : null
                        };
                    }
                }
            }

            return item;
        }

        public void UpdateCartItemQuantity(int cartId, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE  customer.Cart_Items SET quantity = @Quantity WHERE cart_item_id = @CartId", conn);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@CartId", cartId);
                cmd.ExecuteNonQuery();
            }
        }

        //customer Addresses methods

        public List<AddressViewModel> GetAddressesByUserId(int userId)
        {
            var list = new List<AddressViewModel>();
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                string query = "SELECT * FROM customer.Addresses WHERE user_id = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new AddressViewModel
                    {
                        address_id = Convert.ToInt32(reader["address_id"]),
                        user_id = Convert.ToInt32(reader["user_id"]),
                        Name = reader["Name"].ToString(),
                        Street = reader["Street"].ToString(),
                        City = reader["City"].ToString(),
                        ZipCode = reader["ZipCode"].ToString(),
                        Phone = reader["Phone"].ToString()
                    });
                }
            }
            return list;
        }

        public void AddAddress(int userId, AddressViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                string query = @"INSERT INTO customer.Addresses (user_id, Name, Street, City, ZipCode, Phone) 
                             VALUES (@UserId, @Name, @Street, @City, @ZipCode, @Phone)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Street", model.Street);
                cmd.Parameters.AddWithValue("@City", model.City);
                cmd.Parameters.AddWithValue("@ZipCode", model.ZipCode);
                cmd.Parameters.AddWithValue("@Phone", model.Phone);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
