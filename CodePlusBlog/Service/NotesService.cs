using CodePlusBlog.Context;
using CodePlusBlog.IService;
using CodePlusBlog.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CodePlusBlog.Service
{
    public class NotesService : INotesService
    {
        private readonly RepositoryContext _context;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _environment;
        private readonly ICurrentUser _currentUser;
        public NotesService(RepositoryContext context, IFileService fileService, IWebHostEnvironment environment, ICurrentUser currentUser)
        {
            _context = context;
            _fileService = fileService;
            _environment = environment;
            _currentUser = currentUser;
        }

        public async Task<List<NotesDetail>> SaveNotesService(NotesDetail notesDetail)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(notesDetail.Content))
                throw new Exception("Content is null or empty");

            if (string.IsNullOrEmpty(notesDetail.Title))
                throw new Exception("Title is null or empty");

            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var path = "Notes";
                    if (notesDetail.NoteId == 0)
                        await AddNotesDetail(notesDetail, path);
                    else
                        await UpdateNotesDetail(notesDetail, path);

                    transaction.Commit(); // Commit the transaction if successful
                    return await GetAllNotesService();
                }
                catch (Exception)
                {
                    if (File.Exists(notesDetail.FilePath))
                        File.Delete(notesDetail.FilePath);

                    transaction.Rollback();
                    throw;
                }
            }
        }

        private async Task AddNotesDetail(NotesDetail notesDetail, string path)
        {
            var lastNote = await _context.notesDetails.OrderBy(x => x.NoteId).LastOrDefaultAsync();
            if (lastNote == null)
                notesDetail.NoteId = 1;
            else
                notesDetail.NoteId = lastNote.NoteId + 1;

            string notesFileName = "Notes_" + GetRandomNumber();
            _fileService.SaveTextFile(path, notesDetail.Content, notesFileName);
            notesDetail.FilePath = Path.Combine(path, notesFileName + ".txt");
            await _context.notesDetails.AddAsync(notesDetail);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateNotesDetail(NotesDetail notesDetail, string path)
        {
            var note = await _context.notesDetails.FirstOrDefaultAsync(x => x.NoteId == notesDetail.NoteId);
            if (note == null)
                throw new Exception("Notes detail not found");

            string notesFileName = "Notes_" + GetRandomNumber();
            _fileService.SaveTextFile(path, notesDetail.Content, notesFileName);
            note.FilePath = Path.Combine(path, notesFileName + ".txt");
            note.Title = notesDetail.Title;

            await _context.SaveChangesAsync();
        }

        private string GetRandomNumber()
        {
            var rnd = new Random();
            return rnd.Next(ApplicationConstant.MinRandomValue, ApplicationConstant.MaxRandomValue).ToString();
        }

        public async Task<List<NotesDetail>> GetAllNotesService()
        {
            var result = await _context.notesDetails.ToListAsync();
            return result;
        }

        public async Task<NotesDetail> GetNoteByIdService(int noteId)
        {
            var result = await _context.notesDetails.FirstOrDefaultAsync(x => x.NoteId == noteId);
            var filepath = Path.Combine(_environment.WebRootPath, result.FilePath);
            if (File.Exists(filepath))
                result.Content = await File.ReadAllTextAsync(filepath);
            else
                throw new Exception("File not found");
            return result;
        }
    }
}
