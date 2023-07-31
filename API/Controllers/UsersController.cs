using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserService userService, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        // [FromQuery] mean that the parameter is coming from the query string
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            AppUser currentUser = await _userService.GetUserByUsernameAsync(User.GetUsername());
            PagedList<MemberDto> users = await _userService.GetMembersAsync(currentUser, userParams);
            PaginationHeader pgHeader = new(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            Response.AddPaginationHeader(pgHeader);

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userService.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
        {
            // take username from security initialized in TokenService (moved to ClaimsPrincipalExtension)
            // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            return await _userService.SaveAllAsync() 
                ? NoContent() 
                : BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file) 
        {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri, 
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _userService.SaveAllAsync()) 
            {
                // this return 201 response code with Location header point to GetUser(username) API url
                return CreatedAtAction(
                    nameof(GetUser), 
                    new {username = user.UserName}, 
                    _mapper.Map<PhotoDto>(photo)
                );
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId) {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());
            
            if (user == null) return NotFound();

            var newPhoto = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (newPhoto == null) return NotFound();

            if (newPhoto.IsMain) return BadRequest("Is already main");

            var photo = user.Photos.FirstOrDefault(p => p.IsMain == true);

            if (photo != null) photo.IsMain = false;

            newPhoto.IsMain = true;

            return await _userService.SaveAllAsync() 
                ? NoContent() 
                : BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> deletePhoto(int photoId) {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You can not delete main photo");

            if (photo.PublicId != null) 
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            return await _userService.SaveAllAsync() 
                ? Ok() 
                : BadRequest("Failed to delete photo");
        }
    }
}