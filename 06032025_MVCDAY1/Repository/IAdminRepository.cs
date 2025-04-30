namespace _06032025_MVCDAY1.Repository
{
    public interface IAdminRepository<T> where T : class
    {
        IEnumerable<T> GetAllData();
        T GetDataById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
