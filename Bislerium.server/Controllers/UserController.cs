using Bislerium.server.Data.Entities;
using Bislerium.shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;


        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;

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

        [Authorize(Roles ="Blogger")] 
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


        [HttpGet]
        [Authorize(Roles ="Blogger")]
        public IActionResult GetAllUsersWithRoles()
        {
            var usersWithRoles = new List<object>();

            // Get all users
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                var userRoles = _userManager.GetRolesAsync(user).Result;

                usersWithRoles.Add(new
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = userRoles
                });
            }

            return Ok(usersWithRoles);
        }


        [Authorize(Roles = "Blogger")]
        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.Length > 3 * 1024 * 1024)
                return BadRequest("Image size exceeds the limit");


            var fileName = $"{Guid.NewGuid()}_{file.FileName}";

            var uploadPath = Path.Combine("wwwroot", "profile-pictures");

            Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ProfilePictureUrl = filePath;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok("Profile picture uploaded successfully");
            else
                return BadRequest(result.Errors);
        }

    }

}
