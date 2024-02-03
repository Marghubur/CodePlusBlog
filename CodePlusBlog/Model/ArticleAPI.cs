namespace CodePlusBlog.Model
{
    public class ArticleAPI
    {
        public string Status { get; set; }
        public List<Article> Articles { get; set; }
    }

    public class Article
    {
        public string Title { get; set; }
        public DateTime Published_date { get; set; }
        public string Author { get; set; }
        public string Excerpt { get; set; }
        public string Summary { get; set; }
        public string Topic { get; set; }
        public string Media { get; set; }
        public string _id { get; set; }
    }
}
