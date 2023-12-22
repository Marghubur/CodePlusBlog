using CodePlusBlog.Model;

namespace CodePlusBlog.IService
{
    public interface IFileService
    {
        string SaveFile(string path, List<Files> fileDetail, IFormFileCollection files, string oldFileName = null);
        string SaveTextFile(string path, string body, string fileName);
    }
}
