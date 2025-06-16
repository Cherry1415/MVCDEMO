using _06032025_MVCDAY1.Models;
using Humanizer;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
//using Razorpay.Api;
using System.Collections.Generic;
using System.Data;
using System.Net.WebSockets;

namespace _06032025_MVCDAY1.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _constring;

        public ProductRepository(IConfiguration configuration)
        {
            _constring = configuration.GetConnectionString("DefaultConnection");
        }
        public void NewProduct(int userID,Product product, List<ProductImage> images, List<Prod_Attributes> attributes, List<VendorStock> vstock)
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
                cmd.Parameters.AddWithValue("@user_id ", userID);
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
                    cmd.Parameters.AddWithValue("@quantity_available", vst.quantity_available);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

        }

        public int GetStockByProductId(int productId)
        {
            int quantity = 0;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("SELECT quantity_available FROM vendor.stock WHERE product_id = @productId", con);
                cmd.Parameters.AddWithValue("@productId", productId);

                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    quantity = Convert.ToInt32(result);
                }
            }
            return quantity;
        }
        public IEnumerable<Product> GetProducts(int catid,int subcateid)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand sqlCommand = new SqlCommand("vendor.sp_GetProductBysubcate", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@catid", catid);
                sqlCommand.Parameters.AddWithValue("@subcateid", subcateid);
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

        public IEnumerable<Product> VendorGetProducts()
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

        public IEnumerable<Product> VendorOwnProducts(int vendorID)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand sqlCommand = new SqlCommand("vendor.sp_GetOwnProducts", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@vendor_id", vendorID);
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
        public List<Product> GetOutOfStockProducts(int vendorId)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_GetOutOfStockProducts", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vendor_id", vendorId);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    int product_id = Convert.ToInt32(rdr["product_id"]);

                    // Try to find the existing product in the list
                    Product product = products.Find(p => p.product_id == product_id);
                    // If not found, create and add new product
                    if (product == null)
                    {
                        product = new Product
                        {
                            product_id = Convert.ToInt32(rdr["product_id"]),
                            product_name = rdr["product_name"].ToString(),
                            brand_name = rdr["BrandName"].ToString(),
                            category_name = rdr["CategoryName"].ToString(),
                            subcat_name = rdr["SubCategoryName"].ToString(),
                            vendor_id = Convert.ToInt32(rdr["vendor_id"]),
                            price = Convert.ToDecimal(rdr["price"]),
                            ProductImages = new List<ProductImage>(),
                            Prod_Attributes = new List<Prod_Attributes>()
                        };
                        products.Add(product);
                    }
                    // Add image if available
                    if (rdr["prod_img_id"] != DBNull.Value)
                    {
                        ProductImage img = new ProductImage
                        {
                            prod_img_id = Convert.ToInt32(rdr["prod_img_id"]),
                            imgName = rdr["imgName"].ToString(),
                            imgType = rdr["imgType"].ToString(),
                            product_id = Convert.ToInt32(rdr["product_id"])
                        };

                        product.ProductImages.Add(img);
                    }
                    if (rdr["product_desc_id"] != DBNull.Value)
                    {
                        Prod_Attributes prodA = new Prod_Attributes
                        {
                            product_desc_id = Convert.ToInt32(rdr["product_desc_id"]),
                            product_id = Convert.ToInt32(rdr["product_id"]),
                            size = rdr["size"].ToString(),
                            color = rdr["product_id"].ToString(),
                            material = rdr["material"].ToString(),
                            weight = rdr["weight"].ToString(),
                            gender = rdr["gender"].ToString(),
                            capacity = rdr["capacity"].ToString(),
                            display = rdr["display"].ToString(),
                            processor = rdr["processor"].ToString(),
                        };

                        product.Prod_Attributes.Add(prodA);
                    }
                }
            }
            return products;
        }
        public IEnumerable<Product> PendingApproval()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand sqlCommand = new SqlCommand("vendor.sp_pendingApproval", con);
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
                            brand_name = reader["BrandName"].ToString(),
                            category_name = reader["CategoryName"].ToString(),
                            subcat_name = reader["SubCategoryName"].ToString(),
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
        public int GetCategoryIdByName(string catname)
        {
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("SELECT category_id FROM admin.category WHERE name = @catname", con);
                cmd.Parameters.AddWithValue("@catname", catname);
                con.Open();
                return (int?)cmd.ExecuteScalar() ?? 0;
            }
            
        }

        public int GetSubCategoryIdByName(string subcatname)
        {
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("SELECT sub_category_id FROM admin.sub_category WHERE name = @subcatname", con);
                cmd.Parameters.AddWithValue("@subcatname", subcatname);
                con.Open();
                return (int?)cmd.ExecuteScalar() ?? 0;
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            var categories = new List<Category>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("SELECT category_id,name FROM admin.category", con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    categories.Add(new Category
                    {
                        category_id = Convert.ToInt32(rdr["category_id"]),
                        name = rdr["name"].ToString()
                    });
                }
            }
            return categories;
        }

        public IEnumerable<Subcategory> GetSubcategories()
        {
            var subcategories = new List<Subcategory>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("SELECT sub_category_id,name FROM admin.sub_category", con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    subcategories.Add(new Subcategory
                    {
                        sub_category_id = Convert.ToInt32(rdr["sub_category_id"]),
                        name = rdr["name"].ToString()
                    });
                }
            }
            return subcategories;
        }

        public IEnumerable<Brands> GetBrands()
        {
            var brands = new List<Brands>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("SELECT brand_id,name FROM admin.brands", con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    brands.Add(new Brands
                    {
                        brand_id = Convert.ToInt32(rdr["brand_id"]),
                        name = rdr["name"].ToString()
                    });
                }
            }
            return brands;
        }

        public void UpdateProduct(Product product, List<ProductImage> images, List<Prod_Attributes> attributes)
        {
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_UpdateProduct", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@product_id", product.product_id);
                cmd.Parameters.AddWithValue("@product_name", product.product_name);
                cmd.Parameters.AddWithValue("@price", product.price);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            // Update product attributes
            foreach (var attr in attributes)
            {
                using (SqlConnection con = new SqlConnection(_constring))
                {
                    SqlCommand cmd = new SqlCommand("vendor.sp_UpdateProductAttri", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@product_id", product.product_id);
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

            // Update product images
            foreach (var image in images)
            {
                using (SqlConnection con = new SqlConnection(_constring))
                {
                    SqlCommand cmd = new SqlCommand("vendor.sp_UpdateProductImage", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@product_id", product.product_id);
                    cmd.Parameters.AddWithValue("@imgName", image.imgName);
                    cmd.Parameters.AddWithValue("@imgType", image.imgType);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public Product GetProductById(int productId)
        {
            Product product = null;

            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("vendor.sp_GetProductById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productId", productId);

                con.Open();
                //SqlDataReader reader = cmd.ExecuteReader();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (product == null)
                        {
                            product = new Product
                            {
                                product_id = Convert.ToInt32(reader["product_id"]),
                                product_name = reader["product_name"].ToString(),
                                brand_id = Convert.ToInt32(reader["brand_id"]),
                                category_id = Convert.ToInt32(reader["category_id"]),
                                sub_category_id = Convert.ToInt32(reader["subcategory_id"]),
                                price = Convert.ToDecimal(reader["price"]),
                                Prod_Attributes = new List<Prod_Attributes>(),
                                ProductImages = new List<ProductImage>()
                            };

                            // Add only once
                            product.Prod_Attributes.Add(new Prod_Attributes
                            {
                                size = reader["size"].ToString(),
                                color = reader["color"].ToString(),
                                material = reader["material"].ToString(),
                                gender = reader["gender"].ToString(),
                                processor = reader["processor"].ToString(),
                                display = reader["display"].ToString(),
                                capacity = reader["capacity"].ToString(),
                                weight = reader["weight"].ToString()
                            });
                        }

                        // Add each image (can be multiple)
                        product.ProductImages.Add(new ProductImage
                        {
                            prod_img_id = Convert.ToInt32(reader["prod_img_id"]),
                            imgName = reader["imgName"].ToString(),
                            imgType = reader["imgType"].ToString()
                        });
                    }
                }
            }

            return product;
        }

        public List<CategoryWithProductsViewModel> GetHomePageCategoriesWithProducts()
        {
            var list = new List<CategoryWithProductsViewModel>();

            using (SqlConnection con = new SqlConnection(_constring))
            {
                con.Open();

                // 1. Get all categories
                SqlCommand cmd = new SqlCommand("SELECT category_id, name FROM admin.category", con);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var category = new CategoryWithProductsViewModel
                        {
                            category_id = Convert.ToInt32(reader["category_id"]),
                            name = reader["name"].ToString(),
                            Products = new List<Product>() // Initialize product list
                        };
                        list.Add(category);
                    }
                }

                // 2. For each category, get TOP 4 products with one image
                foreach (var cat in list)
                {
                    SqlCommand prodCmd = new SqlCommand(@"
                SELECT TOP 4 
                    p.product_id, 
                    p.product_name, 
                    p.price,
                    (SELECT TOP 1 imgName FROM vendor.prodImages WHERE product_id = p.product_id) AS imgName
                FROM vendor.Products p
                WHERE p.category_id = @cid
                ORDER BY p.product_id DESC", con);

                    prodCmd.Parameters.AddWithValue("@cid", cat.category_id);

                    using (SqlDataReader prodReader = prodCmd.ExecuteReader())
                    {
                        while (prodReader.Read())
                        {
                            var product = new Product
                            {
                                product_id = Convert.ToInt32(prodReader["product_id"]),
                                product_name = prodReader["product_name"].ToString(),
                                price = Convert.ToDecimal(prodReader["price"]),
                                ProductImages = new List<ProductImage>()
                            };
                            //if (prodReader["imgName"] == DBNull.Value)
                            //{
                            //    ProductImage img = new ProductImage
                            //    {
                            //        prod_img_id = Convert.ToInt32(prodReader["prod_img_id"]),
                            //        imgName = prodReader["imgName"].ToString(),
                            //        imgType = prodReader["imgType"].ToString(),
                            //        product_id = Convert.ToInt32(prodReader["product_id"])
                            //    };

                            //    product.ProductImages.Add(img);
                            //}
                            // Add image if exists
                            if (prodReader["imgName"] != DBNull.Value)
                            {
                                product.ProductImages.Add(new ProductImage
                                {
                                    imgName = prodReader["imgName"].ToString(),
                                 //   prod_img_id = Convert.ToInt32(prodReader["prod_img_id"]),
                                 //   product_id = Convert.ToInt32(prodReader["product_id"])
                                });
                            }

                            cat.Products.Add(product);
                        }
                    }
                }

                con.Close();
            }

            return list;
        }

        public List<ProductReview> GetAllReviewsbyvendor(int vendor_id)
        {
            var ratings = new List<ProductReview>();

            using (SqlConnection conn = new SqlConnection(_constring))
            using (SqlCommand cmd = new SqlCommand("customer.sp_getallreviewsbyvendor", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vendorid", vendor_id);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ratings.Add(new ProductReview
                        {
                            Id = (int)reader["review_id"],
                            username = reader["Customer_Name"].ToString(),
                            product_name = reader["product_name"].ToString(),
                            
                            CreatedDate = Convert.ToDateTime(reader["CreateDate"]),
                            Rating = (int)reader["rating"],
                            Review = reader["review"]?.ToString()
                        });
                    }
                }
            }

            return ratings;
        }

        public List<ProductReview> GetReviewsByProductId(int productId)
        {
            List<ProductReview> reviews = new List<ProductReview>();

            using (SqlConnection con = new SqlConnection(_constring))
            {
                string query = @"SELECT cr.review_id,
                 vp.product_name,
                 cc.first_name + ' ' + cc.last_name AS Customer_Name,
                 cr.CreateDate,
                 cr.rating,
                 cr.review
                 FROM customer.Reviews cr
                 INNER JOIN vendor.Products vp ON vp.product_id = cr.product_id
                 INNER JOIN customer.registeruser cc ON cc.user_id = cr.user_id
                 WHERE vp.product_id=@ProductId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reviews.Add(new ProductReview
                    {
                        Id = Convert.ToInt32(reader["review_id"]),
                        //ProductId = Convert.ToInt32(reader["ProductId"]),
                        username = reader["Customer_Name"].ToString(),
                        product_name = reader["product_name"].ToString(),
                        Rating = Convert.ToInt32(reader["rating"]),
                        Review = reader["review"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["CreateDate"])
                    });
                }
            }

            return reviews;
        }

        public IEnumerable<Product> GetFilteredProducts(int catid, int subcateid, List<string> brands, decimal minPrice, decimal maxPrice)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand sqlCommand = new SqlCommand("vendor.sp_GetFilteredProducts", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@catid", catid);
                sqlCommand.Parameters.AddWithValue("@subcateid", subcateid);
                sqlCommand.Parameters.AddWithValue("@minPrice", minPrice);
                sqlCommand.Parameters.AddWithValue("@maxPrice", maxPrice);

                // Convert brand list to comma-separated string
                string brandCsv = brands != null && brands.Any() ? string.Join(",", brands) : null;
                sqlCommand.Parameters.AddWithValue("@brands", (object)brandCsv ?? DBNull.Value);

                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    int product_id = Convert.ToInt32(reader["product_id"]);

                    Product product = products.Find(p => p.product_id == product_id);

                    if (product == null)
                    {
                        product = new Product
                        {
                            product_id = product_id,
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

                    // Add image
                    if (reader["prod_img_id"] != DBNull.Value)
                    {
                        ProductImage img = new ProductImage
                        {
                            prod_img_id = Convert.ToInt32(reader["prod_img_id"]),
                            imgName = reader["imgName"].ToString(),
                            imgType = reader["imgType"].ToString(),
                            product_id = product_id
                        };

                        product.ProductImages.Add(img);
                    }

                    // Add attributes
                    if (reader["product_desc_id"] != DBNull.Value)
                    {
                        Prod_Attributes attr = new Prod_Attributes
                        {
                            product_desc_id = Convert.ToInt32(reader["product_desc_id"]),
                            product_id = product_id,
                            size = reader["size"].ToString(),
                            color = reader["color"].ToString(),
                            material = reader["material"].ToString(),
                            gender = reader["gender"].ToString(),
                            processor = reader["processor"].ToString(),
                            display = reader["display"].ToString(),
                            capacity = reader["capacity"].ToString(),
                            weight = reader["weight"].ToString()
                        };

                        product.Prod_Attributes.Add(attr);
                    }
                }
            }

            return products;
        }


        public List<string> GetAllBrands(int categoryId, int subcategoryId)
        {
            var brands = new List<string>();

            using (SqlConnection conn = new SqlConnection(_constring))
            {
                conn.Open();
                string query = @"SELECT DISTINCT  ab.name
                                FROM vendor.Products vp
                                INNER JOIN admin.brands ab
                                    ON ab.brand_id=vp.brand_id
                                WHERE category_id = @CategoryId 
                                AND subcategory_id = @SubcategoryId
                                ORDER BY ab.name";

                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@CategoryId", categoryId);
                    command.Parameters.AddWithValue("@SubcategoryId", subcategoryId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["name"] != DBNull.Value)
                            {
                                brands.Add(reader["name"].ToString());
                            }
                        }
                    }
                }
            }

            return brands;
        }


        public (decimal,decimal) GetPriceRange(int catid,int subcatid)
        {
            

            using (SqlConnection con = new SqlConnection(_constring))
            {
                string query = "SELECT MIN(price) AS MinPrice, MAX(price) AS MaxPrice FROM vendor.Products WHERE category_id=@catId AND subcategory_id=@subcatId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@catId", catid);
                cmd.Parameters.AddWithValue("@subcatId", subcatid);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    decimal minPrice = reader["MinPrice"] != DBNull.Value ? Convert.ToDecimal(reader["MinPrice"]) : 0;
                    decimal maxPrice = reader["MaxPrice"] != DBNull.Value ? Convert.ToDecimal(reader["MaxPrice"]) : 0;

                    return (minPrice, maxPrice);
                }
            }

            return (0, 0);
        }
        public IEnumerable<Product> TopSellingProduct()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand sqlCommand = new SqlCommand("customer.sp_topsellingproducts", con);
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
                          //  imgType = reader["imgType"].ToString(),
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

        //product perfomance

        public List<ProductPerformanceModel> GetProductPerformance(int vendorId)
        {
            List<ProductPerformanceModel> list = new List<ProductPerformanceModel>();
            using (SqlConnection con = new SqlConnection(_constring))
            {
                SqlCommand cmd = new SqlCommand("sp_GetProductPerformance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VendorId", vendorId);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new ProductPerformanceModel
                    {
                        ProductName = rdr["product_name"].ToString(),
                        TotalOrders = Convert.ToInt32(rdr["TotalOrders"]),
                        TotalRevenue = Convert.ToDecimal(rdr["TotalRevenue"])
                        
                    });
                }
            }
            return list;
        }
    }
}
