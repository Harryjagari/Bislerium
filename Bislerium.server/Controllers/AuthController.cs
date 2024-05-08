using Bislerium.server.Data;
using Bislerium.server.Data.Entities;
using Bislerium.server.Services;
using Bislerium.shared.Dtos;
using Bislerium.shared.Models;
using Bislerium.shared.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Net;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService, IConfiguration configuration, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
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
            await _userManager.AddToRoleAsync(user, "Blogger");

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


        [HttpPost("login")]
        public async Task<ResultWithDataDto<AuthResponseDto>> SigninAsync([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                return GenerateAuthResponse(user, roles);
            }
            return ResultWithDataDto<AuthResponseDto>.Failure("Unauthorized");
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
