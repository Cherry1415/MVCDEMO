using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IUserRepository
    {
        bool Register(User user);
        bool Login(string email, string password);

        List<Product> GetAllProduct();
    }
}
