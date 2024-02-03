using CodePlusBlog.Context;
using CodePlusBlog.IService;
using Microsoft.EntityFrameworkCore;

namespace CodePlusBlog.Service
{
    public class VideosService : IVideosService
    {
        private readonly RepositoryContext _context;

        public VideosService(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<string> GetVideoByFilterService()
        {
            //var result = await _context.videoDetails.ToListAsync();
            return null;
        }
    }
}
