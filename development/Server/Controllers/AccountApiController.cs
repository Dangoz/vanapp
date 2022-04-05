using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using Server.Types;

namespace Server.Controllers
{
  [EnableCors()]
  [ApiController]
  [Route("api/[controller]")]
  public class AccountApiController : ControllerBase
  {
    private readonly UserManager<SiteUser> _userManager;
    private readonly IConfiguration _config;
    private readonly SignInManager<SiteUser> _signInManager;
    private readonly double bcitLatitude = 49.250947159526945;
    private readonly double bcitLongitude = -123.0034739768768;

    private readonly int MAXIMUM_DISTANCE_IN_KM = 50;

    public AccountApiController(UserManager<SiteUser> userManager, IConfiguration config, SignInManager<SiteUser> signInManager)
    {
      _config = config;
      _userManager = userManager;
      _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<ActionResult<object>> Register(RegisterType registerObj)
    {
      var ifExist = await _userManager.Users.AnyAsync(x => x.UserName == registerObj.username.ToLower());

      if (ifExist) return BadRequest("Username already taken, please choose another one.");

      var userObj = new SiteUser
      {
        Age = registerObj.age,
        Gender = registerObj.gender,
        City = registerObj.city,
      };

      userObj.UserName = registerObj.username.ToLower();

      var result = await _userManager.CreateAsync(userObj, registerObj.password);

      if (!result.Succeeded) return BadRequest(result.Errors);

      return Ok(200);
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login(LoginType loginObj)
    {
      var userObj = await _userManager.Users
          .Include(p => p.Photos)
          .SingleOrDefaultAsync(x => x.UserName == loginObj.username.ToLower());

      if (userObj == null) return Unauthorized("Invalid username");

      var result = await _signInManager
          .CheckPasswordSignInAsync(userObj, loginObj.password, false);

      if (!result.Succeeded) return Unauthorized();

      var imgUrl = "";
      var imgPublicId = "";
      if (userObj.Photos.Count() != 0)
      {
        imgUrl = userObj.Photos.Last().Url;
        imgPublicId = userObj.Photos.Last().PublicId;
      }

      var response = new
      {
        username = userObj.UserName,
        age = userObj.Age,
        gender = userObj.Gender,
        city = userObj.City,
        imgUrl = imgUrl,
        imgPublicId = imgPublicId,
        jwt = await GenerateJwt(userObj),
      };

      return response;
    }

    [HttpGet("ip")]
    public async Task<ActionResult<bool>> CheckAddress()
    {

      var httpClient = new HttpClient();
      var ip_string = await httpClient.GetStringAsync("https://api64.ipify.org");

      Console.WriteLine("your IP is: " + ip_string);

      WebClient client = new WebClient();
      string lookup_url = string.Format("http://ip-api.com/json/{0}", ip_string);


      Console.WriteLine("lookup_url");
      Console.WriteLine(lookup_url);
      string json_geo = client.DownloadString(lookup_url);
      Console.WriteLine(json_geo);

      LocationType location = JsonSerializer.Deserialize<LocationType>(json_geo);

      double meter_distance = GetDistance(bcitLongitude, bcitLatitude, location.lon, location.lat);

      Console.WriteLine("longitude");
      Console.WriteLine(location.lon);
      Console.WriteLine("latitude");
      Console.WriteLine(location.lat);
      Console.WriteLine("meter_distance");
      Console.WriteLine(meter_distance);
      Console.WriteLine("distance from center");
      Console.WriteLine(MAXIMUM_DISTANCE_IN_KM * 1000);

      bool within_limit = meter_distance <= MAXIMUM_DISTANCE_IN_KM * 1000 ? true : false;

      return Ok(within_limit);
    }

    private double GetDistance(double longitude, double latitude, double otherLongitude, double otherLatitude)
    {
      var d1 = latitude * (Math.PI / 180.0);
      var num1 = longitude * (Math.PI / 180.0);
      var d2 = otherLatitude * (Math.PI / 180.0);
      var num2 = otherLongitude * (Math.PI / 180.0) - num1;
      var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

      return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }

    private async Task<string> GenerateJwt(SiteUser user)
    {
      var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

      var roles = await _userManager.GetRolesAsync(user);

      claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
      SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(10),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }

  }
}