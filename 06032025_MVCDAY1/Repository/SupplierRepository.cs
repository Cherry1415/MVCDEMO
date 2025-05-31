using _06032025_MVCDAY1.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace _06032025_MVCDAY1.Repository
{
    public class SupplierRepository:ISupplierRepository
    {
        private readonly string _connectionString;

        public SupplierRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SupplierDashboard GetDashboardCounts()
        {
            var dashboard = new SupplierDashboard();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                dashboard.TotalOrders = GetCount(conn, "SELECT COUNT(*) FROM customer.Orders");
                dashboard.VendorCount = GetCount(conn, "SELECT COUNT(*) FROM vendor.Vendors");
                dashboard.WarehouseCount = GetCount(conn, "SELECT COUNT(*) FROM Supplier.WareHouse");
            }

            return dashboard;
        }

        private int GetCount(SqlConnection conn, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                var result = cmd.ExecuteScalar();
                return result != DBNull.Value && result != null ? Convert.ToInt32(result) : 0;
            }
        }
        //supplier contanctus

        public bool AddContactMessage(SupplierContactus contact)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO Supplier.ContactUs (user_id, Message, CreatedAt) VALUES (@user_id, @Message, @CreatedAt)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", contact.user_id);
                    cmd.Parameters.AddWithValue("@Message", contact.Message);
                    cmd.Parameters.AddWithValue("@CreatedAt", contact.CreatedAt);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Optional: log exception
                return false;
            }
        }
        public List<SupplierOrder> GetAllVendorOrders()
        {
            var orders = new List<SupplierOrder>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM customer.Orders";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new SupplierOrder
                    {
                        order_id = (int)reader["order_id"],
                        user_id = (int)reader["user_id"],
                        order_date = reader["order_date"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["order_date"]) : default,
                        required_date = reader["require_date"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["require_date"]) : default,
                        status = reader["status"]?.ToString() ?? string.Empty // Safe fallback
                    });
                }
            }

            return orders;
        }



        
        public bool AddVendor(SupplierVendor vendor)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("insert_vendor", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@seller_name", vendor.seller_name);
                cmd.Parameters.AddWithValue("@user_id", vendor.user_id);
                cmd.Parameters.AddWithValue("PANDetail", vendor.PANDetail);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        public IEnumerable<SupplierVendor> GetAll()
        {
            List<SupplierVendor> vendors = new List<SupplierVendor>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("get_all_vendors", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vendors.Add(new SupplierVendor
                        {
                            vendor_id = reader.GetInt32(0),
                            seller_name = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty,
                            user_id = reader.GetInt32(2),
                            PANDetail = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty
                        });
                    }
                }
            }

            return vendors;
        }

        public SupplierVendor GetVendor(int id)
        {
            SupplierVendor vendor = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("get_vendor_by_id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vendor_id", id);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        vendor = new SupplierVendor
                        {
                            vendor_id = id,
                            seller_name = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty,
                            user_id = reader.GetInt32(2),
                            PANDetail = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty
                        };
                    }
                }
            }

            return vendor;
        }

        public bool AddWareHouse(WareHouse wareHouse)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("insert_warehouse", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@warehouse_name", wareHouse.warehouse_name);
                cmd.Parameters.AddWithValue("@phone", wareHouse.phone);
                cmd.Parameters.AddWithValue("@capacity", wareHouse.capacity);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();

                return rowsAffected > 0; // ✅ Returns true if insertion is successful
            }
        }

        // ✅ Delete Warehouse
        public bool DeleteWareHouse(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("delete_warehouse", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@warehouse_id", id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();

                return rowsAffected > 0;
            }
        }

        // ✅ Update Warehouse
        public bool EditWareHouse(WareHouse wareHouse)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("update_warehouse", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@warehouse_id", wareHouse.warehouse_id);
                cmd.Parameters.AddWithValue("@warehouse_name", wareHouse.warehouse_name);
                cmd.Parameters.AddWithValue("@phone", wareHouse.phone);
                cmd.Parameters.AddWithValue("@capacity", wareHouse.capacity);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();

                return rowsAffected > 0; // ✅ Returns true if update is successful
            }
        }

        // ✅ Get All Warehouses (Fixed `yield return` issue)
        public IEnumerable<WareHouse> GetAllwarehouse()
        {
            List<WareHouse> warehouses = new List<WareHouse>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("get_all_warehouses", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        warehouses.Add(new WareHouse
                        {
                            warehouse_id = reader.GetInt32(0),
                            warehouse_name = reader.GetString(1),
                            phone = reader["phone"].ToString(),
                            capacity = reader.GetInt32(3)
                        });
                    }
                }
            }

            return warehouses; // ✅ Returning after closing the connection
        }

        // ✅ Get Warehouse by ID
        public WareHouse GetbyId(int id)
        {
            WareHouse warehouse = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("get_warehouse_by_id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@warehouse_id", id);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        warehouse = new WareHouse
                        {
                            warehouse_id = Convert.ToInt32(reader["warehouse_id"]),
                            warehouse_name = reader.GetString(1),
                            capacity = reader.GetInt32(2),
                            phone = reader["phone"].ToString()
                        };
                    }
                }
            }
            return warehouse;
        }


        public WareHouse_details GetWareHouse_details(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
        SELECT 
            wd.warehouse_id,
            w.warehouse_name,
            w.phone,
            w.capacity,
            wd.available_quantity,
            wd.supplier_id,
            wd.vendor_id,
            wd.address_id
        FROM Supplier.WareHouse_details wd
        INNER JOIN Supplier.WareHouse w 
        ON wd.warehouse_id = w.warehouse_id
        WHERE wd.warehouse_id = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new WareHouse_details
                    {
                        warehouse_id = (int)reader["warehouse_id"],
                        warehouse_name = reader["warehouse_name"].ToString(),
                        phone = reader["phone"].ToString(),
                        capacity = (int)reader["capacity"],
                        available_quantity = (int)reader["available_quantity"],
                        supplier_id = (int)reader["supplier_id"],
                        vendor_id = (int)reader["vendor_id"],
                        address_id = (int)reader["address_id"]
                    };
                }
            }
            return null;
        }
        public bool AddWareHouseDetails(WareHouse_details details)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("insert_warehouse_details", conn); // your stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@warehouse_id", details.warehouse_id);
                cmd.Parameters.AddWithValue("@available_quantity", details.available_quantity);
                cmd.Parameters.AddWithValue("@supplier_id", details.supplier_id);
                cmd.Parameters.AddWithValue("@vendor_id", details.vendor_id);
                cmd.Parameters.AddWithValue("@address_id", details.address_id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();

                return rowsAffected > 0;
            }
        }

        public bool UpdateOrderStatus(int orderId, string newStatus)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateOrderStatus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters expected by the stored procedure
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    cmd.Parameters.AddWithValue("@newStatus", newStatus);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    return rowsAffected > 0;
                }
            }
        }
        public List<UserOrder> GetOrdersAssignedToSupplier(int supplierId)
        {
            List<UserOrder> list = new List<UserOrder>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"SELECT o.order_id, c.first_name+' '+c.last_name AS CustomerName, o.Status, o.order_date,o.require_date,o.delivery_date,ca.Street+','+ca.City+','+ca.ZipCode AS Customer_Address
                         FROM [customer].[Orders] o
                         INNER JOIN [customer].registeruser c ON o.user_id = c.user_id
						 INNER JOIN customer.Addresses ca ON o.address_id=ca.address_id
                         WHERE o.supplier_id = (
							SELECT supplier_id FROM Supplier.Supplier_tbl WHERE user_id = @SupplierId
						)
						ORDER BY o.order_date DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SupplierId", supplierId);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new UserOrder
                    {
                        Id = Convert.ToInt32(rdr["order_id"]),
                        Customer_name = rdr["CustomerName"].ToString(),
                        Status = rdr["Status"].ToString(),
                        CreatedDate = Convert.ToDateTime(rdr["order_date"]),
                        require_date=Convert.ToDateTime(rdr["require_date"]),
                        delivered_date = rdr["delivery_date"] != DBNull.Value
                 ? Convert.ToDateTime(rdr["delivery_date"])
                 : (DateTime?)null,
                        addressid = rdr["Customer_Address"].ToString()
                    });
                }
            }
            return list;
        }
    }
}
