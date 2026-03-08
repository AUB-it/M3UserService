using Models;

namespace UserService.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUser(UserDTO user);
    Task<User?> GetUserById(Guid id);
    Task<List<User>> GetAllUsers();
    Task<User?> UpdateUser(Guid id, UserDTO user);
    Task<bool> DeleteUser(Guid id);
}