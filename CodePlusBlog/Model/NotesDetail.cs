using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodePlusBlog.Model
{
    public class NotesDetail
    {
        [Key]
        public int NoteId { get; set; }
        public string Title { get; set; }
        [NotMapped]
        public string Content { get; set; }
        public string FilePath { get; set; }
        public int UserId { get; set; }
    }
}
