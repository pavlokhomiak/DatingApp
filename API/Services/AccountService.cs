using System.Security.Cryptography;
using System.Text;
using API.Entities;
using API.Interfaces;

namespace API.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;

    public AccountService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public AppUser Register(AppUser user, string password)
    {
        // HMACSHA512 implements IDisposable
        // 'using' keyword call Dispose() which remove this object after using
        using HMACSHA512 hmac = new HMACSHA512();
        byte[] hmac_salt = hmac.Key;
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        user.PasswordSalt = hmac_salt;
        
        var userFromDb = _userRepository.AddUser(user);
        _userRepository.SaveAllAsync();

        return userFromDb;
    }

    public bool CheckPassword(AppUser user, string password)
    {
        using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return false;
        }

        return true;
    }

    public async Task<bool> UserExists(string username)
    {
        return await _userRepository.UserExists(username);
    }
}