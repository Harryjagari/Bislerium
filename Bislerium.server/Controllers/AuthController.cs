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
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;

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
        private readonly DataContext _context;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService, IConfiguration configuration, TokenService tokenService, DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _tokenService = tokenService;
            _context = context;

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
            // Assign role to the user
            await _userManager.AddToRoleAsync(user, "Blogger");

            // Generate the email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Encode the token for URL usage
            var encodedToken = UrlEncoder.Default.Encode(token);
            // Construct the confirmation link URL
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new { token = encodedToken, email = user.Email }, Request.Scheme);

            if (confirmationLink == null)
            {
                // Log or handle the error
                return BadRequest("Failed to generate confirmation link");
            }

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
        public async Task<IActionResult> SigninAsync([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);


            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);

                var authResponseResult = await GenerateAuthResponse(user);

                if (authResponseResult.IsSuccess)
                {
                    return Ok(authResponseResult.Data);
                }
                else
                {
                    return BadRequest(authResponseResult.ErrorMessage);
                }
            }

            return BadRequest("Unauthorized");
        }

        private async Task<ResultWithDataDto<AuthResponseDto>> GenerateAuthResponse(User user)
        {
            var loggedInUser = new LoggedInUser(user.Id, user.UserName, user.Email, user.Address);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles == null || roles.Count == 0)
            {
                return ResultWithDataDto<AuthResponseDto>.Failure("No roles found for the user.");
            }

            var token = _tokenService.GenerateJwt(loggedInUser, roles);

            var authResponse = new AuthResponseDto(loggedInUser, token, roles);

            return ResultWithDataDto<AuthResponseDto>.Success(authResponse);
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("User logged out successfully.");
        }
    }
}
