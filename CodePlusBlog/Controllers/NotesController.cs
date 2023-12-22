using CodePlusBlog.IService;
using CodePlusBlog.Model;
using CodePlusBlog.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodePlusBlog.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesService)
        {
            _notesService = notesService;
        }

        [HttpGet("GetAllNotes")]
        public async Task<ApiResponse> GetAllNotes()
        {
            var result = await _notesService.GetAllNotesService();
            return new ApiResponse(result);
        }

        [HttpGet("GetNoteById/{noteId}")]
        public async Task<ApiResponse> GetNoteById([FromRoute] int noteId)
        {
            var result = await _notesService.GetNoteByIdService(noteId);
            return new ApiResponse(result);
        }

        [HttpPost("ManageNote")]
        public async Task<ApiResponse> ManageNote([FromBody] NotesDetail notesDetail)
        {
            var result = await _notesService.SaveNotesService(notesDetail);
            return new ApiResponse(result);
        }
    }
}
