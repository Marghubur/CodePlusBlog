using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModalLayer.Model
{
    public class ContentList
    {
        [Key]
        public int ContentId { get; set; }
        public string Type { get; set; }
        public int Part { get; set; }
        public string FilePath { get; set; }
        public int FileId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string ImgPath { get; set; }
        [NotMapped]
        public string ImgId { get; set; }
        [NotMapped]
        public string BodyContent { get; set; }
        public bool IsArticle { get; set; }
        public bool IsPublish { get; set; }
        public DateTime? PublishOn { get; set; }
        public DateTime SaveOn { get; set; }
        public string Tags { get; set; }
        [NotMapped]
        public List<string> AllTags { get; set; }
        public string Author { get; set; }
    }
}
