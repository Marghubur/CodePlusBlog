using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodePlusBlog.Model
{
    [Table("FileDetail")]
    public class Files
    {
        [Required]
        [Key]
        public long FileId { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FilePath { get; set; }
        [Required]
        public string FileExtension { get; set; }
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public string FileNewName { get; set; }
    }
}
