using CodePlusBlog.IService;
using CodePlusBlog.Service;
using Microsoft.AspNetCore.Mvc;

namespace CodePlusBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IVideosService _videosService;
        private readonly IDailyNewsArticleService _dailyNewsArticleService;

        public VideosController(IVideosService videosService, IDailyNewsArticleService dailyNewsArticleService)
        {
            _videosService = videosService;
            _dailyNewsArticleService = dailyNewsArticleService;
        }
        [HttpGet("Scrpping")]
        public async Task<ApiResponse> GetData()
        {
            var result = await _dailyNewsArticleService.GetRandomArticle();
            return new ApiResponse(result);
        }
    }
}
