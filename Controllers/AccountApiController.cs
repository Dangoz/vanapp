using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Server.Interfaces;
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
        public AccountApiController(UserManager<SiteUser> userManager, IConfiguration config, SignInManager<SiteUser> signInManager)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<object>> Register(RegisterType registerObj) {
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
        public async Task<ActionResult<object>> Login(LoginType loginObj) {
            Console.WriteLine("1111111111111111");
            Console.WriteLine(loginObj.username);
            Console.WriteLine(loginObj.password);
            var userObj = await _userManager.Users
                .SingleOrDefaultAsync(x => x.UserName == loginObj.username.ToLower());

            if (userObj == null) return Unauthorized("Invalid username");

            var result = await _signInManager
                .CheckPasswordSignInAsync(userObj, loginObj.password, false);

            if (!result.Succeeded) return Unauthorized();

            var reponse = new {
                username = userObj.UserName,
                age = userObj.Age,
                gender = userObj.Gender,
                city = userObj.City,
                jwt = await GenerateJwt(userObj),
            };

            return reponse;
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

            var tokenDescriptor = new SecurityTokenDescriptor {
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