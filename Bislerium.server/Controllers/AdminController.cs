using Bislerium.server.Data;
using Bislerium.server.Data.Entities;
using Bislerium.server.Services;
using Bislerium.shared.Dtos;
using Bislerium.shared.Models;
using Bislerium.shared.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Encodings.Web;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService, IConfiguration configuration, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a user with the same email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("A admin with this email already exists.");
            }

            var user = new User
            {
                UserName = model.FullName,
                Email = model.Email,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                RegistrationDate = DateTime.UtcNow 
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
            await _userManager.AddToRoleAsync(user, "Admin");

            // Create the email message
            var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink);

            // Send email confirmation email
            _emailService.SendEmail(message);

            return Ok("Admin registered successfully. Please check your email for the confirmation link.");
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


        [HttpPost("login")]
        public async Task<ResultWithDataDto<AuthResponseDto>> AdminSigninAsync([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                return GenerateAuthResponse(user, roles);
            }
            return ResultWithDataDto<AuthResponseDto>.Failure("Unauthorized");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAdmin(string userId)
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

        [Authorize(Roles = "Blogger")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAdminProfile([FromBody] ProfileUpdateModel profileModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            // Update user properties
            user.UserName = profileModel.FullName;
            user.Address = profileModel.Address;
            user.PhoneNumber = profileModel.PhoneNumber;
            user.DateOfBirth = profileModel.DateOfBirth;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok("Profile updated successfully");
            else
                return BadRequest(result.Errors);
        }

        private ResultWithDataDto<AuthResponseDto> GenerateAuthResponse(User user, IEnumerable<string> roles)
        {
            var loggedInUser = new LoggedInUser(user.Id, user.UserName, user.Email, user.Address);
            var token = _tokenService.GenerateJwt(loggedInUser, roles);

            var authResponse = new AuthResponseDto(loggedInUser, token);

            return ResultWithDataDto<AuthResponseDto>.Success(authResponse);
        }

    }
}
