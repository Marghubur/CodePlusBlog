using ModalLayer.Model;

namespace CodePlusBlog.IService
{
    public interface IArticleService
    {
        Task<ContentList> GetContentByIdService(int contentId);
        Task<List<ContentList>> GetContentListService(int page);
        Task<List<ContentList>> GetAllContentListService(int page);
        Task<string> SaveArticleService(ContentList contentList, IFormFileCollection file);
        List<ContentList> GetArticleListService(int page);
        Task<ContentList> PublishArticleService(ContentList contentList);
    }
}
