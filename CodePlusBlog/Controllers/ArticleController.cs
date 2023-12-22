using CodePlusBlog.IService;
using CodePlusBlog.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ModalLayer.Model;
using Newtonsoft.Json;
using System.Net;

namespace CodePlusBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly HttpContext _httpContext;

        public ArticleController(IArticleService articleService, IHttpContextAccessor httpContext)
        {
            _articleService = articleService;
            _httpContext = httpContext.HttpContext;
        }

        [HttpGet("GetContentList/{page}")]
        public async Task<ApiResponse> GetContentList([FromRoute] int page)
        {
            var result = await _articleService.GetContentListService(page);
            return new ApiResponse(result);
        }

        [HttpGet("GetAllContentList/{page}")]
        public async Task<ApiResponse> GetAllContentList([FromRoute] int page)
        {
            var result = await _articleService.GetAllContentListService(page);
            return new ApiResponse(result);
        }

        [HttpGet("GetContentById/{contentId}")]
        public async Task<ApiResponse> GetContentByIdS([FromRoute] int contentId)
        {
            var result = await _articleService.GetContentByIdService(contentId);
            return new ApiResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("SaveArticle")]
        public async Task<ApiResponse> SaveArticle()
        {
            try
            {
                StringValues article = default(string);
                _httpContext.Request.Form.TryGetValue("article", out article);
                if (article.Count > 0)
                {
                    IFormFileCollection file = _httpContext.Request.Form.Files;
                    ContentList contentList = JsonConvert.DeserializeObject<ContentList>(article);
                    var result = await _articleService.SaveArticleService(contentList, file);
                    return new ApiResponse(result);
                }
                else
                {
                    return new ApiResponse("Fail", (int)HttpStatusCode.BadRequest);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("GetArticleList/{page}")]
        public ApiResponse GetArticleList([FromRoute] int page)
        {
            var result = _articleService.GetArticleListService(page);
            return new ApiResponse(result);
        }

        [HttpPost("PublishArticle")]
        public ApiResponse PublishArticle([FromBody] ContentList contentList)
        {
            var result = _articleService.PublishArticleService(contentList);
            return new ApiResponse(result);
        }
    }
}
