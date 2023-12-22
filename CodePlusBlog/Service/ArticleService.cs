using CodePlusBlog.Context;
using CodePlusBlog.IService;
using CodePlusBlog.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ModalLayer.Model;
using Newtonsoft.Json;

namespace CodePlusBlog.Service
{
    public class ArticleService : IArticleService
    {
        private readonly RepositoryContext _context;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _environment;
        public ArticleService(RepositoryContext context, IFileService fileService, IWebHostEnvironment environment)
        {
            _context = context;
            _fileService = fileService;
            _environment = environment;
        }

        public async Task<string> SaveArticleService(ContentList contentList, IFormFileCollection files)
        {
            string result = string.Empty;
            long fileId = 0;
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var path = "Airticle";
                    if (files != null && files.Count > 0)
                    {
                        var fileDetail = files.Select(x => new Files
                        {
                            FileName = x.Name,
                            FilePath = path,
                            FileId = fileId,
                        }).ToList<Files>();
                        _fileService.SaveFile(path, fileDetail, files, fileDetail[0].FileName);
                        contentList.ImgPath = Path.Combine(path, fileDetail[0].FileName);
                    }
                    _fileService.SaveTextFile(path, contentList.BodyContent, contentList.Type + "_" + contentList.Part);
                    contentList.FilePath = Path.Combine(path, contentList.Type + "_" + contentList.Part + ".txt");
                    if (contentList.ContentId == 0)
                        result = await AddArcticleDetail(contentList);
                    else
                        result = await UpdateArcticleDetail(contentList);

                    transaction.Commit(); // Commit the transaction if successful
                    return result;
                }
                catch (Exception)
                {
                    if (File.Exists(contentList.ImgPath))
                        File.Delete(contentList.ImgPath);

                    if (File.Exists(contentList.FilePath))
                        File.Delete(contentList.FilePath);

                    transaction.Rollback();
                    throw;
                }
            }
        }

        private async Task<string> AddArcticleDetail(ContentList contentList)
        {
            var lastContent = await _context.contentlist.OrderBy(x => x.ContentId).LastOrDefaultAsync();
            if (lastContent == null)
                contentList.ContentId = 1;
            else
                contentList.ContentId = lastContent.ContentId + 1;

            contentList.IsPublish = false;
            contentList.SaveOn = DateTime.Now;
            if (contentList.AllTags != null && contentList.AllTags.Count > 0)
                contentList.Tags = JsonConvert.SerializeObject(contentList.AllTags);
            else
                contentList.Tags = "[]";

            await _context.contentlist.AddAsync(contentList);
            await _context.SaveChangesAsync();
            return "content added successfully";
        }

        private async Task<string> UpdateArcticleDetail(ContentList contentList)
        {
            var content = await _context.contentlist.FirstOrDefaultAsync(x => x.ContentId == contentList.ContentId);
            if (content == null)
                throw new Exception("Content detail not found");

            content.Part = contentList.Part;
            content.Type = contentList.Type;
            content.Title = contentList.Title;
            content.Detail = contentList.Detail;
            content.IsArticle = contentList.IsArticle;
            if (contentList.AllTags != null && contentList.AllTags.Count > 0)
                content.Tags = JsonConvert.SerializeObject(contentList.AllTags);

            content.SaveOn = DateTime.Now;

            await _context.SaveChangesAsync();
            return "content updated successfully";
        }

        public async Task<ContentList> GetContentByIdService(int contentId)
        {
            string text = string.Empty;
            if (contentId <= 0)
                throw new Exception("Invalid content id");

            ContentList content = await _context.contentlist.FirstOrDefaultAsync(x => x.ContentId == contentId);
            if (content == null)
                throw new Exception("Content detail not founf");
            if (!string.IsNullOrEmpty(content.Tags))
                content.AllTags = JsonConvert.DeserializeObject<List<string>>(content.Tags);

            var filepath = Path.Combine(_environment.WebRootPath, content.FilePath);
            if (File.Exists(filepath))
                content.BodyContent = await File.ReadAllTextAsync(filepath);
            else
                throw new Exception("File not found");

            return content;
        }

        public async Task<List<ContentList>> GetContentListService(int page)
        {
            var result = await _context.contentlist.ToListAsync();
            result = result.Where(x => x.IsPublish).Skip((page - 1) * ApplicationConstant.PageSize).Take(ApplicationConstant.PageSize).ToList();
            return result;
        }

        public List<ContentList> GetArticleListService(int page)
        {
            var result = _context.contentlist.Where(x => x.IsArticle).ToList();
            result = result.Where(x => x.IsPublish).Skip((page - 1) * ApplicationConstant.PageSize).Take(ApplicationConstant.PageSize).ToList();
            return result;
        }

        public async Task<List<ContentList>> GetAllContentListService(int page)
        {
            var result = await _context.contentlist.ToListAsync();
            result = result.Skip((page - 1) * ApplicationConstant.PageSize).Take(ApplicationConstant.PageSize).ToList();
            return result;
        }

        public async Task<ContentList> PublishArticleService(ContentList contentList)
        {
            var content = await _context.contentlist.FirstOrDefaultAsync(x => x.ContentId == contentList.ContentId);
            if (content == null)
                throw new Exception("Content detail not found");
            content.IsPublish = contentList.IsPublish;
            if (content.IsPublish)
                content.PublishOn = DateTime.Now;

            await _context.SaveChangesAsync();
            return content;
        }
    }
}
