using CodePlusBlog.IService;
using CodePlusBlog.Service;
using Microsoft.AspNetCore.Mvc;

namespace CodePlusBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyRandonArticleController : ControllerBase
    {
        private readonly IDailyNewsArticleService _dailyNewsArticleService;

        public DailyRandonArticleController(IDailyNewsArticleService dailyNewsArticleService)
        {
            _dailyNewsArticleService = dailyNewsArticleService;
        }
        [HttpGet("GetDailyRandomArticle")]
        public async Task<ApiResponse> GetDailyRandomArticle()
        {
            var result = await _dailyNewsArticleService.GetRandomArticle();
            return new ApiResponse(result);
        }
    }
}
