using Bislerium.server.Data.Entities;
using Bislerium.shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Bislerium.server.Data;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        private readonly DataContext _context;
        public UserController(UserManager<User> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Retrieve comments associated with the user
            var userComments = _context.Comments.Where(c => c.AuthorId == userId);
            // Retrieve blog posts associated with the user
            var userBlogPosts = _context.BlogPosts.Where(b => b.AuthorId == userId);

            var userReactions = _context.Reactions.Where(b => b.UserId == userId);


            // Delete blog posts associated with the user
            _context.BlogPosts.RemoveRange(userBlogPosts);

            // Delete comments associated with the user
            _context.Comments.RemoveRange(userComments);

            _context.Reactions.RemoveRange(userReactions);

            // Delete the user
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                // Handle delete failure
                return StatusCode(500, "Failed to delete user.");
            }
        }




        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateModel profileModel)
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


        [HttpGet]
        [Authorize]
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


        //[Authorize(Roles = "Blogger")]
        [HttpPost("upload-profile-picture")]
        [Authorize]
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
