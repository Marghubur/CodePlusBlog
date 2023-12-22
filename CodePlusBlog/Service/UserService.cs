using CodePlusBlog.Context;
using CodePlusBlog.IService;
using CodePlusBlog.Model;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Service;

namespace CodePlusBlog.Service
{
    public class UserService : IUserService
    {
        private readonly RepositoryContext _context;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(RepositoryContext context, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponse> AuthenticateUserService(User user)
        {
            if (string.IsNullOrEmpty(user.Email))
                throw new Exception("Please enter email first");

            if (string.IsNullOrEmpty(user.Password))
                throw new Exception("Please enter password first");

            User userDetail = await _context.user.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userDetail == null)
                throw new Exception("Email is not match");

            string password = Util.DeEncryptPassword(userDetail.Password);
            if (password != user.Password)
                throw new Exception("Password is not match");

            LoginResponse loginResponse = new LoginResponse();
            loginResponse.Token = Util.GenerateJWTToken(userDetail);

            var response = _httpContextAccessor.HttpContext.Response;
            response.Cookies.Append("jwt", loginResponse.Token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
            loginResponse.user = userDetail;
            if (userDetail.UsertypeId == 1)
                loginResponse.menu = await _context.menu.ToListAsync();
            else
                loginResponse.menu = _context.menu.Where(x => x.AccessCode == 0).ToList();

            return loginResponse;
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
                await _emailService.SendEmail("marghub12@gmail.com", "Forgot password", $"Your new Password is {password}");
                return "Password is send in your register email";

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<string> RegisterUserService(User user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Email))
                    throw new Exception("Please enter email first");

                if (string.IsNullOrEmpty(user.Password))
                    throw new Exception("Please enter password first");

                if (string.IsNullOrEmpty(user.UserName))
                    throw new Exception("Please enter username first");

                var userDetail = await _context.user.FirstOrDefaultAsync(x => x.Email == user.Email);
                if (userDetail != null)
                    throw new Exception("Email id is already registered");

                var lastUser = await _context.user.OrderBy(x => x.UserId).LastOrDefaultAsync();
                if (lastUser == null)
                    user.UserId = 1;
                else
                    user.UserId = lastUser.UserId + 1;

                user.Password = Util.EncryptPassword(user.Password);
                user.UsertypeId = 0;
                await _context.user.AddAsync(user);
                await _context.SaveChangesAsync();
                return "Registration successfully";

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
