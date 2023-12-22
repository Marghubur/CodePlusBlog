using CodePlusBlog.Model;

namespace CodePlusBlog.IService
{
    public interface INotesService
    {
        Task<NotesDetail> GetNoteByIdService(int noteId);
        Task<List<NotesDetail>> GetAllNotesService();
        Task<List<NotesDetail>> SaveNotesService(NotesDetail notesDetail);
    }
}
