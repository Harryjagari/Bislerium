using Bislerium.server.Data.Entities;
using Bislerium.shared.Dtos;
using Bislerium.shared.Models;
using Bislerium.shared.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bislerium.server.Data;
using System.Text.Encodings.Web;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a user with the same email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("A user with this email already exists.");
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                RegistrationDate = DateTime.UtcNow // Set registration date to current UTC date/time
            };

            // Create the user
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Generate the email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Encode the token for URL usage
            var encodedToken = UrlEncoder.Default.Encode(token);
            // Construct the confirmation link URL
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "User", new { token = encodedToken, email = user.Email }, Request.Scheme);

            if (confirmationLink == null)
            {
                // Log or handle the error
                return BadRequest("Failed to generate confirmation link");
            }


            // Assign role to the user
            await _userManager.AddToRoleAsync(user, "Blogger"); // Assign the "Blogger" role by default

            // Create the email message
            var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink);

            // Send email confirmation email
            _emailService.SendEmail(message);

            return Ok("User registered successfully. Please check your email for the confirmation link.");
        }



        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                token = WebUtility.UrlDecode(token);
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                      new Response { Status = "Success", Message = "Email Verified Successfully" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                       new Response { Status = "Error", Message = "This User Doesnot exist!" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }


                var jwtToken = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });
                //returning the token...

            }
            return Unauthorized();


        }

        [Authorize(Roles = "Blogger")] 
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return NoContent();
            else
                return BadRequest(result.Errors);
        }

        [Authorize(Roles ="Blogger")] // Example: Only authenticated users can update their profile
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateModel profileModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            // Update user properties
            user.FullName = profileModel.FullName;
            user.Address = profileModel.Address;
            user.PhoneNumber = profileModel.PhoneNumber;
            user.DateOfBirth = profileModel.DateOfBirth;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok("Profile updated successfully");
            else
                return BadRequest(result.Errors);
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }

}
