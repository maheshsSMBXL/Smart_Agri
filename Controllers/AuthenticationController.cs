using Agri_Smart.data;
using Agri_Smart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Agri_Smart.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _dbcontext;

        public AuthenticationController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration,
            SignInManager<IdentityUser> signInManager, IApplicationDbContext dbcontext)
        { 
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _dbcontext = dbcontext;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterUser registerUser) 
        {
            //check user exist
            var userExist = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == registerUser.PhoneNumber);
            //var userExist = await _userManager.FindByNameAsync(registerUser.UserName);
            if (userExist != null) 
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User Already Exist." });
            }
            //Save user in database
            IdentityUser user = new()
            {
                UserName = registerUser.UserName,
                Email = registerUser.Email,
                PhoneNumber = registerUser.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            var token = GenerateJwtToken(user);
            return result.Succeeded 
                ? StatusCode(StatusCodes.Status201Created,
                    new Response { Status = "Success", Message = "User Created Successfully", Token = token })
                : StatusCode(StatusCodes.Status500InternalServerError,  
                    new Response { Status = "Error", Message = "User Failed to Created" });            
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (user == null)
            {
                return Ok(new Response { Status = "Error", Message = "Log In Failed" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Ok(new Response { Status = "Error", Message = "Log In Failed"});
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new Response { Status = "Success", Message = "User LoggedIn Successfully", Token = token });
        }
        [HttpPost]
        [Route("ValidateMobileNumber")]
        public async Task<IActionResult> ValidateMobileNumber([FromBody]PhoneNumber phoneNumber)
        {
            //check user exist
            var userExist = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber.phoneNumber);
            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber.phoneNumber);
            //var userExist = await _userManager.FindByNameAsync(registerUser.UserName);
            if (userExist != null)
            {
                var ExistingUserToken = GenerateJwtToken(phoneNumber.phoneNumber);
                return Ok(new { Status = "Success", Message = "User Already Exist.", Token = ExistingUserToken, OnBoardStatus = userInfo?.OnBoardingStatus });
            }
            //Save user in database
            IdentityUser user = new()
            {
                UserName = phoneNumber.phoneNumber,
                PhoneNumber = phoneNumber.phoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await _userManager.CreateAsync(user);
            var token = GenerateJwtToken(phoneNumber.phoneNumber);
            return result.Succeeded
                ? Ok(new { Status = "Success", Message = "User Created Successfully", Token = token, OnBoardStatus = userInfo?.OnBoardingStatus })
                : Ok(new { Status = "Error", Message = "User Failed to Created" });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.PhoneNumber),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                //issuer: _configuration["Jwt:Issuer"],
                //audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateJwtToken(string mobileNumber)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, mobileNumber),
                    new Claim("MobileNumber", mobileNumber) // Adding mobile number claim
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        [Authorize]
        [HttpDelete("DeleteUser/{Reason}")]
        public async Task<IActionResult> DeleteUser(string Reason)
        {
            // Get the mobile number from the claims
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;

            // Find the user with the given mobile number
            var user = await _userManager.Users.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Attempt to delete the user
            var result = await _userManager.DeleteAsync(user);


            if (result.Succeeded)
            {
                if (userInfo != null)
                {
                    _dbcontext.UserInfo.Remove(userInfo);
                    _dbcontext.SaveChanges();

                    var deletedUser = new DeletedUsers();
                    deletedUser.Id = Guid.NewGuid();
                    deletedUser.PhoneNumber = mobileNumber;
                    deletedUser.Name = userInfo.Name;
                    deletedUser.Email = userInfo.Email;
                    deletedUser.Reason = Reason;
                    deletedUser.UserCreatedDate = userInfo.UserCreatedDate;
                    deletedUser.UserDeletedDate = DateTime.Now;
                    _dbcontext.DeletedUsers.Add(deletedUser);
                    _dbcontext.SaveChanges();
                }
                return Ok(new { message = "User deleted successfully" });
            }

            // If deletion fails, return a failure response
            return StatusCode(500, new { message = "Error deleting user", errors = result.Errors });
        }
    }
}
