using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);

    Task<User?> GetUserByIdAsync(int id);

    Task AddUserAsync(User user);

    Task SaveChangesAsync();
}