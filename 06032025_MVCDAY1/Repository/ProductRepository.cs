using _06032025_MVCDAY1.Models;
using Humanizer;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _06032025_MVCDAY1.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _constring;

        public ProductRepository(IConfiguration configuration)
        {
            _constring = configuration.GetConnectionString("DefaultConnection");
        }
        public void NewProduct(Product product, List<ProductImage> images, List<Prod_Attributes> attributes, List<VendorStock> vstock)
        {
            int productId;
            using (SqlConnection conn = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_AddProduct", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@product_name", product.product_name);
                cmd.Parameters.AddWithValue("@brand_id", product.brand_id);
                cmd.Parameters.AddWithValue("@category_id", product.category_id);
                cmd.Parameters.AddWithValue("@subcategory_id", product.sub_category_id);
                cmd.Parameters.AddWithValue("@vendor_id", product.vendor_id);
                cmd.Parameters.AddWithValue("@price", product.price);

                SqlParameter outputId = new SqlParameter("@product_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outputId);

                conn.Open();
                cmd.ExecuteNonQuery();
                productId = (int)outputId.Value;
                conn.Close();
            }
            foreach (var attr in attributes)
            {
                using (SqlConnection con = new SqlConnection(_constring))
                {
                    SqlCommand cmd = new SqlCommand("vendor.sp_AddProductAttri", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@product_id", productId);
                    cmd.Parameters.AddWithValue("@size", attr.size ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@color", attr.color);
                    cmd.Parameters.AddWithValue("@material", attr.material);
                    cmd.Parameters.AddWithValue("@weight", attr.weight ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@gender", attr.gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@capacity", attr.capacity ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@display", attr.display ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@processor", attr.processor ?? (object)DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            foreach (var image in images)
            {
                using (SqlConnection con = new SqlConnection(_constring))
                {
                    SqlCommand cmd = new SqlCommand("vendor.sp_AddProductImage", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@imgName", image.imgName);
                    cmd.Parameters.AddWithValue("@imgType", image.imgType);
                    cmd.Parameters.AddWithValue("@product_id", productId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            foreach (var vst in vstock)
            {
                using (SqlConnection con = new SqlConnection(_constring))
                {
                    SqlCommand cmd = new SqlCommand("vendor.sp_AddStock", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@product_id", productId);
                    cmd.Parameters.AddWithValue("@quantity_avilable", vst.quantity_available);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

        }

        public IEnumerable<Product> GetProducts()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand sqlCommand = new SqlCommand("vendor.sp_GetProducts", con);
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
                            prod_img_id = Convert.ToInt32(reader["prod_img_id"]),
                            imgName = reader["imgName"].ToString(),
                            imgType = reader["imgType"].ToString(),
                            product_id = Convert.ToInt32(reader["product_id"])
                        };

                        product.ProductImages.Add(img);
                    }
                    if (reader["product_desc_id"] != DBNull.Value)
                    {
                        Prod_Attributes prodA = new Prod_Attributes
                        {
                            product_desc_id = Convert.ToInt32(reader["product_desc_id"]),
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

        
    }
}
