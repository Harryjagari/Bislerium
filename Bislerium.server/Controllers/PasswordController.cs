﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Bislerium.server.Data.Entities;
using Bislerium.shared.Models;
using Bislerium.shared.Services;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public PasswordController(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return BadRequest("User does not exist");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.OldPassword);

            if (!passwordValid)
            {
                return BadRequest("Old password is incorrect");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password reset successfully");
            }
            else
            {
                return BadRequest("Failed to reset password");
            }
        }


        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return Ok("If the email exists in our system, we have sent a password reset OTP to it.");
            }

            var otp = GenerateOTP();

            user.ResetPasswordOTP = otp;
            user.ResetPasswordOTPIssueTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            var message = new Message(new string[] { user.Email }, "Password Reset OTP", $"Your OTP to reset the password is: {otp}");

             _emailService.SendEmail(message);

            return Ok("If the email exists in our system, we have sent a password reset OTP to it.");
        }

        [HttpPost("reset-passwordWithOTP")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordWithOTPDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return BadRequest("Invalid request");
            }

            if (user.ResetPasswordOTPIssueTime != null && user.ResetPasswordOTP == dto.OTP && user.ResetPasswordOTPIssueTime.Value.AddMinutes(15) >= DateTime.UtcNow)
            {
                var result = await _userManager.ResetPasswordAsync(user, dto.OTP, dto.NewPassword);

                if (result.Succeeded)
                {
                    user.ResetPasswordOTP = null;
                    user.ResetPasswordOTPIssueTime = null;
                    await _userManager.UpdateAsync(user);
                    return Ok("Password reset successfully");
                }
            }

            return BadRequest("Invalid OTP or OTP has expired");
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
