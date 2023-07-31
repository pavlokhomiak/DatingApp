using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserService
{
    Task<AppUser> GetUserByUsernameAsync(string username);
    Task<PagedList<MemberDto>> GetMembersAsync(AppUser currentUser, UserParams userParams);
    Task<MemberDto> GetMemberAsync(string username);
    Task<bool> SaveAllAsync();

}