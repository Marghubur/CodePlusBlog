using CodePlusBlog.Context;
using CodePlusBlog.CRONService;
using CodePlusBlog.IService;
using CodePlusBlog.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServiceLayer.Service;

namespace CodePlusBlog.Service
{
    public class UserService : IUserService
    {
        private readonly RepositoryContext _context;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        public UserService(RepositoryContext context, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IMemoryCache cache)
        {
            _context = context;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        public async Task<LoginResponse> AuthenticateUserService(User user)
        {
            validateAuthenticateUser(user);
            User userDetail = await GetUserByEmail(user.Email);

            LoginResponse loginResponse = new LoginResponse();
            if (!userDetail.PasswordChangeRequired)
            {
                string password = Util.DeEncryptPassword(userDetail.Password);
                if (password != user.Password)
                    throw new Exception("Password is not match");

                loginResponse.Menu = await GetMenu(userDetail.UsertypeId);
                loginResponse.Token = Util.GenerateJWTToken(userDetail);
                SetJWTCookie(loginResponse.Token);
            }

            loginResponse.User = userDetail;
            return loginResponse;
        }

        private async Task<List<Menu>> GetMenu(int usertypeid)
        {
            List<Menu> menus = null;
            if (usertypeid == 1)
                menus = await _context.menu.ToListAsync();
            else
                menus = await _context.menu.Where(x => x.AccessCode == 0).ToListAsync();

            return menus;
        }

        private void SetJWTCookie(string token)
        {
            var response = _httpContextAccessor.HttpContext.Response;
            response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
        }

        private void validateAuthenticateUser(User user)
        {
            if (string.IsNullOrEmpty(user.Email))
                throw new Exception("Please enter email first");

            if (string.IsNullOrEmpty(user.Password))
                throw new Exception("Please enter password first");
        }

        private async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.user.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (user == null)
                throw new Exception("Email is not registered");

            return user;
        }

        public async Task<string> ForgotPasswordService(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    throw new Exception("Invalid email");

                var user = await _context.user.FirstOrDefaultAsync(x => x.Email.Equals(email));
                if (user == null)
                    throw new Exception("Email is not registered");

                string password = Util.DeEncryptPassword(user.Password);
                await _emailService.SendEmail(email, "Forgot password", $"Your Password is {password}");
                return "Password is send in your register email";

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> ChangePasswordService(User user)
        {
            try
            {
                ValidateChangePasswordUser(user);
                var existingUser = await _context.user.FirstOrDefaultAsync(x => x.UserId == user.UserId);
                if (existingUser == null)
                    throw new Exception("User detail not found");

                string password = Util.DeEncryptPassword(existingUser.Password);
                if (!password.Equals(user.Password))
                    throw new Exception("Old password is not matched");

                VerifyOtp(user.Email, user.OTP);

                existingUser.Password = Util.EncryptPassword(user.NewPassword);
                existingUser.PasswordChangeRequired = false;
                await _context.SaveChangesAsync();

                return "Password is changed successfully";

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> GenerateOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
                return "Invalid email";

            //string otp = new Random().Next(100000, 999999).ToString();
            //_cache.Set(email, otp, new MemoryCacheEntryOptions
            //{
            //    AbsoluteExpiration = DateTime.Now.AddMinutes(99) // Set expiration time to 5 minutes
            //});

            //_otpCleanupService.TrackCacheEntry(userId);
            //await _emailService.SendEmail(email, "OTP", $"Your OTP is {otp}. It will be valid for 5 minutes only");
            return "OTP is send on your registered email";
        }

        private string VerifyOtp(string email, string enteredOtp)
        {
            if (_cache.TryGetValue<string>(email, out var storedOtp))
            {
                // Check if the OTP has expired
                if (_cache.TryGetValue<DateTime>($"_expires_{email}", out var expirationTime) && expirationTime < DateTime.Now)
                {
                    _cache.Remove(email);
                    _cache.Remove($"_expires_{email}");
                    //_otpCleanupService.RemoveTrackedEntry(userId);
                    throw new Exception("OTP has expired. Please generate a new OTP.");
                }

                if (enteredOtp == storedOtp)
                {
                    _cache.Remove(email);
                    _cache.Remove($"_expires_{email}");
                    //_otpCleanupService.RemoveTrackedEntry(userId);
                    return "OTP verification successful. User logged in.";
                }
            }

            throw new Exception("Invalid OTP or expired. Please try again.");
        }

        private void ValidateChangePasswordUser(User user)
        {
            if (string.IsNullOrEmpty(user.Password))
                throw new Exception("Invalid password");

            if (string.IsNullOrEmpty(user.NewPassword))
                throw new Exception("Invalid new password");

            if (string.IsNullOrEmpty(user.Email))
                throw new Exception("Invalid email");

            if (string.IsNullOrEmpty(user.OTP))
                throw new Exception("Invalid otp");

            if (user.UserId == 0)
                throw new Exception("Invalid user");

            if (user.NewPassword.Equals(user.Password))
                throw new Exception("Old password and new password is same");
        }

        public async Task<string> RegisterUserService(User user)
        {
            try
            {
                ValidateRegisterUser(user);
                var userDetail = await GetUserByEmail(user.Email);
                var lastUser = await _context.user.OrderBy(x => x.UserId).LastOrDefaultAsync();
                if (lastUser == null)
                    user.UserId = 1;
                else
                    user.UserId = lastUser.UserId + 1;

                user.Password = Util.EncryptPassword(ApplicationConstant.TempPassword);
                user.UsertypeId = 0;
                await _context.user.AddAsync(user);
                await _context.SaveChangesAsync();
                await _emailService.SendEmail(user.Email, "Temporaary password", $"Your Password is {ApplicationConstant.TempPassword}");

                return "Registration successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ValidateRegisterUser(User user)
        {
            if (string.IsNullOrEmpty(user.Email))
                throw new Exception("Please enter email first");

            if (string.IsNullOrEmpty(user.UserName))
                throw new Exception("Please enter username first");
        }
    }
}