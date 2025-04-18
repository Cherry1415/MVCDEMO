using _06032025_MVCDAY1.Models;
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
    }
}
