
using _06032025_MVCDAY1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _06032025_MVCDAY1.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;

        public AdminRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        //auditlog methods
        public void Log(AdminAuditLog log)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [admin].[AuditLogs]
                            (UserName, Action, TableName, RecordId, OldValues, NewValues)
                             VALUES (@UserName, @Action, @TableName, @RecordId, @OldValues, @NewValues)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserName", log.userid);
                cmd.Parameters.AddWithValue("@Action", log.Action);
                cmd.Parameters.AddWithValue("@TableName", log.TableName);
                cmd.Parameters.AddWithValue("@RecordId", log.RecordId);
                cmd.Parameters.AddWithValue("@OldValues", (object)log.OldValues ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NewValues", (object)log.NewValues ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<AdminAuditLog> GetAllLogs()
        {
            List<AdminAuditLog> logs = new List<AdminAuditLog>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [admin].[AuditLogs] ORDER BY Timestamp DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    logs.Add(new AdminAuditLog
                    {
                        Id = (int)reader["Id"],
                        userid = (int)reader["user_id"],
                        Action = reader["Action"].ToString(),
                        TableName = reader["TableName"].ToString(),
                        RecordId = (int)reader["RecordId"],
                        OldValues = reader["OldValues"]?.ToString(),
                        NewValues = reader["NewValues"]?.ToString(),
                        Timestamp = Convert.ToDateTime(reader["Timestamp"])
                    });
                }
            }
            return logs;
        }
        //all category methods
        public void Add(Category category)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO admin.category (name, description, status, created_at) 
                                 VALUES (@name, @description, @status, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", category.name);
                cmd.Parameters.AddWithValue("@description", category.description ?? "");
                cmd.Parameters.AddWithValue("@status", category.status);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void Update(Category category)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"UPDATE admin.category 
                         SET name = @name, description = @description, status = @status, updated_at = GETDATE()
                         WHERE category_id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", category.name);
                    cmd.Parameters.AddWithValue("@description", category.description ?? "");
                    cmd.Parameters.AddWithValue("@status", category.status);
                    cmd.Parameters.AddWithValue("@id", category.category_id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM admin.category WHERE category_id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Category> GetAll()
        {
            var list = new List<Category>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT category_id, name, description, status, created_at,updated_at
                         FROM admin.category";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Category
                        {
                            category_id = (int)reader["category_id"],
                            name = reader["name"].ToString(),
                            description = reader["description"].ToString(),
                            status = reader["status"].ToString(),
                            created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"],
                             updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"]
                        });
                    }
                }
            }
            return list;
        }
        public Category GetById(int id)
        {
            Category category = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT category_id, name, description, status, created_at, updated_at FROM admin.category WHERE category_id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    category = new Category
                    {
                        category_id = Convert.ToInt32(reader["category_id"]),
                        name = reader["name"].ToString(),
                        description = reader["description"].ToString(),
                        status = reader["status"].ToString(),
                      //  created_at = reader["created_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["created_at"]),
                      //  updated_at = reader["updated_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["updated_at"])
                    };
                }
            }
            return category;
        }

        public List<AdminOrderViewModel> GetAllOrders(string status)
        {
            List<AdminOrderViewModel> orders = new List<AdminOrderViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                SELECT distinct O.order_id,c.first_name+' '+c.last_name as [customername], O.order_date, O.Status
                FROM customer.Orders O
                JOIN customer.registeruser C 
                ON O.user_id = C.user_id
                JOIN customer.Order_Items OI 
                ON O.order_id = OI.order_id
                WHERE 1=1";

                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    query += " AND O.Status= @Status";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(status) && status != ".0All")
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                }

                
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        orders.Add(new AdminOrderViewModel
                        {
                            OrderId = reader.GetInt32(0),
                            CustomerName = reader.GetString(1),
                            OrderDate = reader.GetDateTime(2),
                            status = reader.GetString(3),
                          //  TotalAmount = reader.GetDecimal(4)
                        });
                    }
                }
            }

            return orders;
        }
        public List<AdminProductViewModel> GetAllProducts()
        {
            var products = new List<AdminProductViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                SELECT P.product_id, P.product_name, C.name AS Category,sc.name as SubCategory, P.Price
                FROM vendor.Products P
                JOIN admin.category C 
                ON P.category_id= C.category_id
                JOIN admin.sub_category sc
                ON p.subcategory_id=sc.sub_category_id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        products.Add(new AdminProductViewModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Category = reader.GetString(2),
                            subcategory=reader.GetString(3),
                            Price = reader.GetDecimal(4)
                            
                        });
                    }
                }
            }

            return products;
        }

        

        public DashboardViewModel GetDashboardData()
        {
            var model = new DashboardViewModel();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Order status counts
                using (SqlCommand cmd = new SqlCommand("SELECT Status, COUNT(*) FROM customer.Orders GROUP BY Status", conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string status = reader.GetString(0);
                        int count = reader.GetInt32(1);

                        model.TotalOrders += count;

                        switch (status.ToLower())
                        {
                            case "pending": model.PendingCount = count; break;
                            case "delivered": model.DeliveredCount = count; break;
                            case "paid": model.CompletedCount = count; break;
                            case "rejected": model.RejectCount = count; break;
                        }
                    }
                    reader.Close();
                }

                // Top 5 selling products
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 5 P.product_name, SUM(OI.Quantity) AS TotalQty
                FROM customer.Order_Items OI
                JOIN vendor.Products P 
                ON OI.product_id= P.product_id
                GROUP BY P.product_name
                ORDER BY TotalQty DESC", conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.TopProducts.Add(new TopProduct
                        {
                            Name = reader.GetString(0),
                            Count = reader.GetInt32(1)
                        });
                    }
                    reader.Close();
                }
            }

            return model;
        }

        //all subcategory methods

        public List<Subcategory> GetAllSubCategory()
        {
            List<Subcategory> list = new List<Subcategory>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT sc.*,ac.name as categoryname FROM admin.sub_category sc INNER JOIN admin.category ac ON sc.category_id=ac.category_id ";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Subcategory
                    {
                        sub_category_id = (int)reader["sub_category_id"],
                        name = reader["name"].ToString(),
                        description = reader["description"].ToString(),
                        category_id = (int)reader["category_id"],
                        status = reader["status"].ToString(),
                        created_at = (DateTime)reader["created_at"],
                        updated_at = (DateTime)reader["updated_at"],
                        category_name = reader["categoryname"].ToString()
                        
                    });
                }
            }
            return list;
        }

        public Subcategory subcategoryGetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM admin.sub_category WHERE sub_category_id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Subcategory
                    {
                        sub_category_id = (int)reader["sub_category_id"],
                        name = reader["name"].ToString(),
                        description = reader["description"].ToString(),
                        category_id = (int)reader["category_id"],
                        status = reader["status"].ToString(),
                        created_at = (DateTime)reader["created_at"],
                        updated_at = (DateTime)reader["updated_at"]
                    };
                }
            }
            return null;
        }

        public void Savesubcategory(Subcategory subcategory)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = subcategory.sub_category_id == 0
                    ? @"INSERT INTO admin.sub_category (name, description, category_id, status, created_at, updated_at)
                   VALUES (@name, @description, @category_id, @status, GETDATE(), GETDATE())"
                    : @"UPDATE admin.sub_category 
                   SET name = @name, description = @description, category_id = @category_id, status = @status, updated_at = GETDATE() 
                   WHERE sub_category_id = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", subcategory.name);
                cmd.Parameters.AddWithValue("@description", subcategory.description);
                cmd.Parameters.AddWithValue("@category_id", subcategory.category_id);
                cmd.Parameters.AddWithValue("@status", subcategory.status);
                if (subcategory.sub_category_id != 0)
                    cmd.Parameters.AddWithValue("@id", subcategory.sub_category_id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Deletesubcategory(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM admin.sub_category WHERE sub_category_id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Product> GetVendorProductApproval()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("vendor.sp_GetVendorProductForApproval", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    int product_id = Convert.ToInt32(reader["product_id"]);

                    // Try to find the existing product in the list
                    Product product = products.Find(p => p.product_id == product_id);

                    // If not found, create and add new product
                    if (product == null)
                    {
                        product = new Product
                        {
                            product_id = Convert.ToInt32(reader["product_id"]),
                            product_name = reader["product_name"].ToString(),
                            category_name = reader["Category_name"].ToString(),
                            brand_name = reader["Brand_name"].ToString(),
                            subcat_name = reader["SubCategory_name"].ToString(),
                            brand_id = Convert.ToInt32(reader["brand_id"]),
                            category_id = Convert.ToInt32(reader["category_id"]),
                            sub_category_id = Convert.ToInt32(reader["subcategory_id"]),
                            vendor_id = Convert.ToInt32(reader["vendor_id"]),
                            price = Convert.ToDecimal(reader["price"]),
                            ProductImages = new List<ProductImage>(),
                            Prod_Attributes = new List<Prod_Attributes>()
                        };

                        products.Add(product);
                    }

                    // Add image if available
                    if (reader["prod_img_id"] != DBNull.Value)
                    {
                        ProductImage img = new ProductImage
                        {
                           // prod_img_id = Convert.ToInt32(reader["prod_img_id"]),
                            imgName = reader["imgName"].ToString(),
                           // imgType = reader["imgType"].ToString(),
                            product_id = Convert.ToInt32(reader["product_id"])
                        };

                        product.ProductImages.Add(img);
                    }
                    if (reader["product_desc_id"] != DBNull.Value)
                    {
                        Prod_Attributes prodA = new Prod_Attributes
                        {
                          //  product_desc_id = Convert.ToInt32(reader["product_desc_id"]),
                            product_id = Convert.ToInt32(reader["product_id"]),
                            size = reader["size"].ToString(),
                            color = reader["product_id"].ToString(),
                            material = reader["material"].ToString(),
                            weight = reader["weight"].ToString(),
                            gender = reader["gender"].ToString(),
                            capacity = reader["capacity"].ToString(),
                            display = reader["display"].ToString(),
                            processor = reader["processor"].ToString(),

                        };

                        product.Prod_Attributes.Add(prodA);
                    }
                }
            }
            return products;
        }

        public void ApproveProduct(int productId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_UpdateApproval", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@productid", productId);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void RejectProduct(int productId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_RejectProductToDelete", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@productid", productId);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public List<AdminPaymentViewModel> GetAllPaymentsWithOrders()
        {
            List<AdminPaymentViewModel> list = new List<AdminPaymentViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT 
                            p.pay_id AS PaymentId,
                            p.amount,
                            p.payment_status,
                            
                            p.PaidOn,
                            o.order_id AS OrderId,
                            o.user_id AS UserId,
                            o.TotalAmount,
                            o.RazorPayOrderId
                         FROM customer.Payment p
                         INNER JOIN customer.Orders o ON p.RazorpayOrderId = o.RazorPayOrderId
                         ORDER BY p.PaymentId";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    AdminPaymentViewModel model = new AdminPaymentViewModel
                    {
                        PaymentId = Convert.ToInt32(reader["PaymentId"]),
                        Amount = Convert.ToDecimal(reader["amount"]),
                        PaymentStatus = reader["payment_status"].ToString(),
                        
                        PaidOn = Convert.ToDateTime(reader["PaidOn"]),
                        OrderId = Convert.ToInt32(reader["OrderId"]),
                        UserId = Convert.ToInt32(reader["UserId"]),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        RazorpayOrderId = reader["RazorPayOrderId"].ToString()
                    };

                    list.Add(model);
                }
            }

            return list;
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            List<OrderItem> items = new List<OrderItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT 
                            oi.item_id,
                            oi.product_id,
                            oi.quantity,
                            oi.price,
                            p.product_name,
                            pi.imgName
                         FROM customer.Order_Items oi
                         INNER JOIN vendor.Products p ON oi.product_id = p.product_id
						 OUTER APPLY (
                            SELECT TOP 1 imgName,prod_img_id
                            FROM vendor.prodImages 
                            WHERE product_id = p.product_id
                            ORDER BY prod_img_id ASC
                            ) pi
                         WHERE oi.order_id = @OrderId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrderItem item = new OrderItem
                    {
                        Id = Convert.ToInt32(reader["item_id"]),
                        ProductId = Convert.ToInt32(reader["product_id"]),
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        Price = Convert.ToDecimal(reader["price"]),
                        product_name = reader["product_name"].ToString(),
                        ImgName = reader["imgName"].ToString()
                    };

                    items.Add(item);
                }
            }

            return items;
        }

        public List<UserOrder> GetFilteredOrders(DateTime? fromDate, DateTime? toDate, string orderStatus)
        {
            List<UserOrder> orders = new List<UserOrder>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
            SELECT order_id, user_id, order_date, status, RazorPayOrderId, TotalAmount, require_date, 
                    PaymentId
            FROM customer.Orders
            WHERE (@fromDate IS NULL OR order_date >= @fromDate)
              AND (@toDate IS NULL OR order_date <= @toDate)
              AND (@orderStatus IS NULL OR status = @orderStatus)", conn);

                cmd.Parameters.AddWithValue("@fromDate", (object)fromDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@toDate", (object)toDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@orderStatus", string.IsNullOrEmpty(orderStatus) ? DBNull.Value : (object)orderStatus);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new UserOrder
                    {
                        Id = reader["order_id"] != DBNull.Value ? Convert.ToInt32(reader["order_id"]) : 0,
                        UserId = reader["user_id"] != DBNull.Value ? Convert.ToInt32(reader["user_id"]) : 0,
                        CreatedDate = reader["order_date"] != DBNull.Value ? Convert.ToDateTime(reader["order_date"]) : DateTime.MinValue,
                        Status = reader["status"] != DBNull.Value ? reader["status"].ToString() : string.Empty,
                        RazorpayOrderId = reader["RazorPayOrderId"] != DBNull.Value ? reader["RazorPayOrderId"].ToString() : string.Empty,
                        TotalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : 0,
                        require_date = reader["require_date"] != DBNull.Value ? (DateTime?)reader["require_date"] : null,
                        // Uncomment and apply same logic for below fields if needed:
                        // OrderStatus = reader["order_status"] != DBNull.Value ? reader["order_status"].ToString() : string.Empty,
                        // CancelReason = reader["cancelreason"] != DBNull.Value ? reader["cancelreason"].ToString() : string.Empty,
                        PaymentId = reader["PaymentId"] != DBNull.Value ? reader["PaymentId"].ToString() : string.Empty
                    });
                }
            }
            return orders;
        }

        public List<WeekSalesViewModel> GetWeeklySales(int month, int year)
        {

            List<WeekSalesViewModel> result = new List<WeekSalesViewModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetWeeklySalesByMonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.Add(new WeekSalesViewModel
                    {
                        WeekNumber = Convert.ToInt32(rdr["WeekNumber"]),
                        TotalSales = Convert.ToDecimal(rdr["TotalSales"])
                    });
                }
            }
            return result;
        }

        public AdminPayoutSummary GetPayoutSummary(int month, int year)
        {
            var weeklyData = GetWeeklySales(month, year); // your method

            decimal totalSales = weeklyData.Sum(w => w.TotalSales);
            decimal payout = totalSales * 0.8m;     // e.g., 70% payout
            decimal pending = totalSales - payout;

            return new AdminPayoutSummary
            {
                MonthName = new DateTime(year, month, 1).ToString("MMMM"),
                Weekly = weeklyData,
                TotalSales = totalSales,
                Payout = payout,
                Pending = pending
            };
        }
    }
}
