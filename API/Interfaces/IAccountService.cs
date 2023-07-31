using API.Entities;

namespace API.Interfaces;

public interface IAccountService
{
    AppUser Register(AppUser user, string password);
    bool CheckPassword(AppUser user, string password);
    Task<bool> UserExists(string username);
}