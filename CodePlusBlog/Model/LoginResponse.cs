namespace CodePlusBlog.Model
{
    public class LoginResponse
    {
        public User User { get; set; }
        public string Token { get; set; }
        public List<Menu> Menu { get; set; }
    }
}
