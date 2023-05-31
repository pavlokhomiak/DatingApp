using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService; 
        }

        [HttpPost("register")]
        // auto bind params from request body obj {"username": "bob", "password": "password"}
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            if (await UserExists(registerDto.Username)) {
                return BadRequest("Username is taken");
            }

            // HMACSHA512 implements IDisposable
            // 'using' keyword call Dispose() which remove this object after using
            using HMACSHA512 hmac = new HMACSHA512();
            byte[] hmac_salt = hmac.Key;

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac_salt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto 
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                // PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) 
        {
            var user = await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid username");

            using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++) 
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDto 
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url
            };
        }

        private async Task<bool> UserExists(string username) 
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}