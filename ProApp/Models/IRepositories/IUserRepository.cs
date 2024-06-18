using ProApp.Models.Entities;

namespace ProApp.Models.IRepositories
{
    public interface IUserRepository
    {
        Task AddUser(User user);
        Task<User> FindUserByUsername(string userName);
    }
}
