using CodePlusBlog.IService;
using CodePlusBlog.Model;
using CodePlusBlog.Service;
using Microsoft.AspNetCore.Mvc;

namespace CodePlusBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<ApiResponse> AuthenticateUser(User user)
        {
            var result = await _userService.AuthenticateUserService(user);
            return new ApiResponse(result);
        }

        [HttpPost("UserRegistration")]
        public async Task<ApiResponse> UserRegistration(User user)
        {
            var result = await _userService.RegisterUserService(user);
            return new ApiResponse(result);
        }

        [HttpGet("ForgotPassword/{email}")]
        public async Task<ApiResponse> ForgotPassword([FromRoute] string email)
        {
            var result = await _userService.ForgotPasswordService(email);
            return new ApiResponse(result);
        }

        [HttpGet("GenerateOTP/{email}")]
        public async Task<ApiResponse> GenerateOTP([FromRoute] string email)
        {
            var result = await _userService.GenerateOtp(email);
            return new ApiResponse(result);
        }

        [HttpPost("ChangePassword")]
        public async Task<ApiResponse> ChangePassword([FromBody] User user)
        {
            var result = await _userService.ChangePasswordService(user);
            return new ApiResponse(result);
        }
    }
}
