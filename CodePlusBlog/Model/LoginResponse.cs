namespace CodePlusBlog.Model
{
    public class LoginResponse
    {
        public User user { get; set; }
        public string Token { get; set; }
        public List<Menu> menu { get; set; }
    }
}
