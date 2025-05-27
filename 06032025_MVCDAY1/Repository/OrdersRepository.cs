using _06032025_MVCDAY1.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;

namespace _06032025_MVCDAY1.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly string _constring;

        public OrdersRepository(IConfiguration configuration)
        {
            _constring = configuration.GetConnectionString("DefaultConnection");
        }
        public VendorStock GetVendorStockById(int id)
        {
            VendorStock vs = new VendorStock();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_GetStockById", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@stock_id", id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    vs.stock_id = Convert.ToInt32(rdr["stock_id"]);
                    vs.product_id = Convert.ToInt32(rdr["product_id"]);
                    vs.stock_status = rdr["stock_status"].ToString();
                    vs.stock_in_date = Convert.ToDateTime(rdr["stock_in_date"]);
                    vs.quantity_available = Convert.ToInt32(rdr["quantity_available"]);
                    vs.restock_date = rdr["restock_date"] != DBNull.Value ? Convert.ToDateTime(rdr["restock_date"]) : DateTime.MinValue;
                    //vs.restock_qty = Convert.ToInt32(rdr["restock_qty"]);
                    //vs.reorder_cost = Convert.ToDecimal(rdr["reorder_cost"]);
                }
            }
            return vs;
        }

        public IEnumerable<VendorStock> manageInventoty(int vendorId)
        {
            using SqlConnection con = new SqlConnection(_constring);
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_manageInventory", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vendorId", vendorId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<VendorStock> stocks = new List<VendorStock>();
                while (reader.Read())
                {
                    VendorStock stock = new VendorStock
                    {
                        stock_id = Convert.ToInt32(reader["stock_id"]),
                        product_id = Convert.ToInt32(reader["product_id"]),
                        quantity_available = Convert.ToInt32(reader["quantity_available"]),
                        stock_in_date = Convert.ToDateTime(reader["stock_in_date"]),
                        restock_date = reader["restock_date"] != DBNull.Value ? Convert.ToDateTime(reader["restock_date"]) : DateTime.MinValue,
                        stock_status = reader["stock_status"].ToString()
                    };
                    stocks.Add(stock);
                }
                con.Close();
                return stocks;
            }
        }
        public IEnumerable<VendorStock> GetShortageItems(int threshold)
        {

            List<VendorStock> alertList = new List<VendorStock>();

            using (SqlConnection conn = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_GetLowStockAlertsByThreshold", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@threshold", threshold);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    alertList.Add(new VendorStock
                    {
                        stock_id = Convert.ToInt32(reader["stock_id"]),
                        product_id = Convert.ToInt32(reader["product_id"]),
                        quantity_available =Convert.ToInt32 (reader["quantity_available"]),
                        product_name = reader["product_name"].ToString()

                    });
                }
            }

            return alertList;
        }
        public void UpdateVendorStock(VendorStock vs)
        {
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_UpdateStock", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@stock_id", vs.stock_id);
                cmd.Parameters.AddWithValue("@quantity_available", vs.quantity_available);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public UserOrder CreateOrder(int userId,decimal totalAmount, string razorpayOrderId, List<OrderItem> items,int addressid)
        {

            using (var connection = new SqlConnection(_constring))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    //if (items == null || !items.Any())
                    //    throw new Exception("No order items provided.");

                    string orderQuery = "INSERT INTO customer.Orders (user_id,TotalAmount, RazorPayOrderId, status, order_date,address_id,require_date) " +
                                        "VALUES (@UserId,@TotalAmount, @RazorpayOrderId, 'Pending', GETDATE(),@addressid,DATEADD(DAY, 3, GETDATE())); SELECT SCOPE_IDENTITY();";

                    var orderCmd = new SqlCommand(orderQuery, connection, transaction);
                    orderCmd.Parameters.AddWithValue("@UserId", userId);
                    //orderCmd.Parameters.AddWithValue("@AddressId", addressId);
                    orderCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    orderCmd.Parameters.AddWithValue("@RazorpayOrderId", razorpayOrderId);
                    orderCmd.Parameters.AddWithValue("@addressid", addressid);
                    int orderId = Convert.ToInt32(orderCmd.ExecuteScalar());

                    foreach (var item in items)
                    {
                        /*if (item.ProductId <= 0 || item.Quantity <= 0 || item.Price <= 0)
                            throw new Exception("Invalid order item data.");*/

                        string itemQuery = "INSERT INTO customer.Order_Items (order_id, product_id, quantity, price) " +
                                           "VALUES (@OrderId, @ProductId, @Quantity, @Price)";

                        var itemCmd = new SqlCommand(itemQuery, connection, transaction);
                        itemCmd.Parameters.AddWithValue("@OrderId", orderId);
                        itemCmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                        itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        itemCmd.Parameters.AddWithValue("@Price", item.Price);

                        itemCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    return new UserOrder
                    {
                        Id = orderId,
                        UserId = userId,
                        TotalAmount = totalAmount,
                        RazorpayOrderId = razorpayOrderId,
                        Status = "Pending"
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Order creation failed: " + ex.Message);
                }
            }
        }


        public void UpdateOrderStatus(string razorpayOrderId, string status)
        {
            using (var connection = new SqlConnection(_constring))
            {
                string query = "UPDATE customer.Orders SET status = @Status WHERE RazorPayOrderId = @RazorpayOrderId";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RazorpayOrderId", razorpayOrderId);
                command.Parameters.AddWithValue("@Status", status);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdatePaymentIdINUser(string razorpayOrderId, string paymentId)
        {
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                string query = "UPDATE customer.Orders SET PaymentId = @PaymentId WHERE RazorpayOrderId = @RazorpayOrderId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PaymentId", paymentId);
                cmd.Parameters.AddWithValue("@RazorpayOrderId", razorpayOrderId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<UserOrder> GetUserOrdersWithItemsAndImages(int userId)
        {
            var orders = new List<UserOrder>();

            using (SqlConnection conn = new SqlConnection(_constring))
            {
                conn.Open();

                string query = @"
                    SELECT DISTINCT 
                     o.order_id AS OrderId,
                    o.TotalAmount,
                    o.Status,
                    o.order_date,
                    o.address_id,
                    oi.product_id,
                    oi.Quantity,
                    oi.Price,
                    p.product_name,
                    pi.imgName
                    FROM customer.Orders o
                    INNER JOIN customer.Order_Items oi ON o.order_id = oi.order_id
                    INNER JOIN vendor.Products p ON oi.product_id = p.product_id
                    OUTER APPLY (
                        SELECT TOP 1 imgName
                        FROM vendor.prodImages 
                        WHERE product_id = p.product_id
                        ORDER BY prod_img_id ASC
                    ) pi
                    WHERE o.user_id = @UserId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int orderId = Convert.ToInt32(reader["OrderId"]);
                        var order = orders.FirstOrDefault(o => o.Id == orderId);
                        if (order == null)
                        {
                            order = new UserOrder
                            {
                                Id = orderId,
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                Status = reader["status"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["order_date"]),
                                addressid = reader["address_id"].ToString(),
                                OrderItems = new List<OrderItem>()
                            };
                            orders.Add(order);
                        }

                        order.OrderItems.Add(new OrderItem
                        {
                            ProductId = Convert.ToInt32(reader["product_id"]),
                            Quantity = Convert.ToInt32(reader["quantity"]),
                            Price = Convert.ToDecimal(reader["price"]),
                            product_name = reader["product_name"].ToString(),
                            ImgName = reader["imgName"]?.ToString() // 👈 Add this property to OrderItem model
                        });
                    }
                }
            }

            return orders;
        }


        public void ClearCart(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                string query = "DELETE FROM customer.Cart_Items WHERE user_id = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public UserOrder GetUserOrders(int userId)
        {
            UserOrder userorders = null;
            using( SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_getOrderByUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
               
                while (rdr.Read())
                {
                    userorders = new UserOrder
                    {
                        Id = Convert.ToInt32(rdr["order_id"]),
                        UserId = Convert.ToInt32(rdr["user_id"]),
                        RazorpayOrderId = rdr["RazorPayOrderId"].ToString(),
                        TotalAmount = Convert.ToDecimal(rdr["TotalAmount"]),
                        Status = rdr["status"].ToString(),
                        CreatedDate = Convert.ToDateTime(rdr["order_date"]),
                        OrderItems = new List<OrderItem>()
                    };
                    userorders.OrderItems.Add(new OrderItem
                    {
                        Id = Convert.ToInt32(rdr["order_item_id"]),
                        ProductId = Convert.ToInt32(rdr["product_id"]),
                        Quantity = Convert.ToInt32(rdr["quantity"]),
                        Price = Convert.ToDecimal(rdr["price"]),
                       product_name = rdr["product_name"].ToString()
                    });
                    
                }
            }

            return userorders;
        }

        public List<UserOrder> GetOrdersWithoutSupplier()
        {
            List<UserOrder> list = new List<UserOrder>();

            using (SqlConnection con = new SqlConnection(_constring))
            {
                string query = @"SELECT o.order_id,u.first_name +' '+u.last_name AS CustomerName, o.Status
                         FROM [customer].[Orders] o
                         INNER JOIN [customer].registeruser u ON o.user_id = u.user_id
                         WHERE o.Status='Paid' AND o.supplier_id IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new UserOrder
                    {
                        Id = Convert.ToInt32(rdr["order_id"]),
                        Customer_name= rdr["CustomerName"].ToString(),
                        Status = rdr["Status"].ToString()
                    });
                }
            }
            return list;
        }
        public List<SupplierViewModel> GetAllSuppliers()
        {
            List<SupplierViewModel> list = new List<SupplierViewModel>();

            using (SqlConnection con = new SqlConnection(_constring))
            {
                string query = "SELECT supplier_id,supplier_name FROM Supplier.Supplier_tbl";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new SupplierViewModel
                    {
                        SupplierId = Convert.ToInt32(rdr["supplier_id"]),
                        Name = rdr["supplier_name"].ToString()
                    });
                }
            }
            return list;
        }
        public bool AssignSupplierToOrder(int orderId, int supplierId)
        {
            using (SqlConnection con = new SqlConnection(_constring))
            {
                string query = "UPDATE [customer].[Orders] SET supplier_id = @SupplierId WHERE order_id = @OrderId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                cmd.Parameters.AddWithValue("@OrderId", orderId);

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }
    }
}
