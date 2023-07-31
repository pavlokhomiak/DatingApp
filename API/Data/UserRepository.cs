using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;

        public UserRepository(DataContext db, IMapper mapper) 
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _db.Users
                .Where(user => user.UserName == username)
                // dont need .Include(p => p.Photos) because ProjectTo() handle it by itself
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            IQueryable<AppUser> query = _db.Users.AsQueryable();

            query = query.Where(u => 
                u.UserName != userParams.CurrentUsername 
                && u.Gender == userParams.Gender);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(u => 
                u.DateOfBirth >= minDob 
                && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider), 
                userParams.PageNumber, 
                userParams.PageSize
            );    
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            // if entity with this primary key is being tracked by the context, it is returned immediately without request to db
            return await _db.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _db.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<List<AppUser>> GetUsersAsync()
        {
            return await _db.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public Task<int> SaveAllAsync()
        {
            return _db.SaveChangesAsync();
        }

        public void Update(AppUser user)
        {
            _db.Entry(user).State = EntityState.Modified;
        }
        
        public AppUser AddUser(AppUser user)
        {
            return _db.Users.Add(user).Entity;
        }
        
        public Task<bool> UserExists(string username) 
        {
            return _db.Users.AnyAsync(user => user.UserName == username);
        }
    }
}