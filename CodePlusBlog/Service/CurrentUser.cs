using CodePlusBlog.Model;

namespace CodePlusBlog.Service
{
    public interface ICurrentUser
    {
        User GetUser();
    }

    public class CurrentUser: ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public User GetUser()
        {
            // Retrieve user information from HttpContext or JWT token
            User user = (User)_httpContextAccessor.HttpContext.Items["User"];

            // Fetch user details from your data store or external service
            // Example: var user = userRepository.GetUserById(userId);

            return user;
        }
    }
}
