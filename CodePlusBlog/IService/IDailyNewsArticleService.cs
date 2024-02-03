using CodePlusBlog.Model;

namespace CodePlusBlog.IService
{
    public interface IDailyNewsArticleService
    {
        Task GetTodayNewsService();
        Task<DailyArticle> scapingText();
        Task<DailyArticle> GetRandomArticle();
    }
}
