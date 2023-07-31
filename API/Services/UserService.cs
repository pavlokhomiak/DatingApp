using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return _userRepository.GetUserByUsernameAsync(username);
    }

    public Task<PagedList<MemberDto>> GetMembersAsync(AppUser currentUser, UserParams userParams)
    {
        userParams.CurrentUsername = currentUser.UserName;
        if (string.IsNullOrEmpty(userParams.Gender)) 
        {
            userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
        }
        return _userRepository.GetMembersAsync(userParams);
    }

    public Task<MemberDto> GetMemberAsync(string username)
    {
        // var user = await _userRepository.GetUserByUsernameAsync(username);
        // return _mapper.Map<MemberDto>(user);
        return _userRepository.GetMemberAsync(username);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _userRepository.SaveAllAsync() > 0;
    }
}