using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Microsoft.AspNetCore.Authorization;
using Server.Services;
using System.Security.Claims;
using Server.Extensions;

namespace Server.Controllers
{
  [EnableCors()]
  [ApiController]
  [Route("api/[controller]")]
  public class UsersApiController : ControllerBase
  {
    private readonly UserRepository _userRepository;
    private readonly PhotoService _photoService;

    public UsersApiController(UserManager<SiteUser> userManager, UserRepository userRepository, PhotoService photoService)
    {
      _userRepository = userRepository;
      _photoService = photoService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
      var users = await _userRepository.GetMembersAsync();

      return Ok(users);
    }

    [Authorize]
    [HttpPost("img")]
    public async Task<ActionResult<object>> AddPhoto(IFormFile file)
    {

      var user = await _userRepository.GetUserByUsernameAsync(User.FindFirst(ClaimTypes.Name)?.Value);

      var result = await _photoService.AddPhotoAsync(file);

      if (result.Error != null) return BadRequest(result.Error.Message);

      var photo = new Photo
      {
        Url = result.SecureUrl.AbsoluteUri,
        PublicId = result.PublicId
      };

      user.Photos.Add(photo);

      if (await _userRepository.SaveAllAsync())
      {
        return new JsonResult(new
        {
          imgUrl = photo.Url,
          imgPublicId = photo.PublicId
        });
      }

      return BadRequest("Problem adding photo");
    }
  }
}