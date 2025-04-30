
using Microsoft.Data.SqlClient;
using System.Data;

namespace _06032025_MVCDAY1.Repository
{
    public class AdminRepository<T> : IAdminRepository<T> where T : class, new()
    {
        private readonly string _connectionString;

        public AdminRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public void Add(T entity)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string procedure = $"admin.Insert{typeof(T).Name}";
                SqlCommand cmd = new SqlCommand(procedure, conn) { CommandType = CommandType.StoredProcedure };

                var keyProperty = typeof(T).GetProperties()
                    .FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));


                foreach (var prop in typeof(T).GetProperties())
                {
                    if (keyProperty != null && prop.Name == keyProperty.Name)
                        continue;

                    var value = prop.GetValue(entity);
                    if (value is DateTime dateTimeValue && dateTimeValue == DateTime.MinValue)
                    {
                        value = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue("@" + prop.Name, value ?? DBNull.Value);
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string procedure = $"admin.Delete{typeof(T).Name}";
                SqlCommand cmd = new SqlCommand(procedure, conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<T> GetAllData()
        {
            var item = new List<T>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string procedure = $"admin.GetAll{typeof(T).Name}";
                SqlCommand cmd = new SqlCommand(procedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    T entity = Activator.CreateInstance<T>();
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                        {
                            prop.SetValue(entity, reader[prop.Name]);
                        }
                    }
                    item.Add(entity);
                }
            }
            return item;
        }

        public T GetDataById(int id)
        {
            T entity = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string procedure = $"admin.Get{typeof(T).Name}ById";
                SqlCommand cmd = new SqlCommand(procedure, conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    entity = Activator.CreateInstance<T>();
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                        {
                            prop.SetValue(entity, reader[prop.Name]);
                        }
                    }
                }
            }
            return entity;
        }

        public void Update(T entity)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string procedure = $"admin.Update{typeof(T).Name}";
                SqlCommand cmd = new SqlCommand(procedure, conn) { CommandType = CommandType.StoredProcedure };

                foreach (var prop in typeof(T).GetProperties())
                {
                    var value = prop.GetValue(entity);
                    if (value is DateTime dateTimeValue && dateTimeValue == DateTime.MinValue)
                    {
                        value = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue("@" + prop.Name, value ?? DBNull.Value);
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
