using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(
            IAccountService accountService, 
            IUserService userService,
            ITokenService tokenService, 
            IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
            _accountService = accountService;
            _tokenService = tokenService; 
        }

        [HttpPost("register")]
        // auto bind params from request body obj {"username": "bob", "password": "password"}
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            bool isUserExists = await _accountService.UserExists(registerDto.Username);
            if (isUserExists) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();
            var userFromDb = _accountService.Register(user, registerDto.Password);

            return new UserDto 
            {
                Username = userFromDb.UserName,
                Token = _tokenService.CreateToken(userFromDb),
                KnownAs = userFromDb.KnownAs,
                Gender = userFromDb.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userService.GetUserByUsernameAsync(loginDto.Username);
            if (user == null) return Unauthorized("Invalid username");
            
            bool isPasswordValid = _accountService.CheckPassword(user, loginDto.Password);
            if (!isPasswordValid) return Unauthorized("Invalid password");

            return new UserDto 
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
    }
}