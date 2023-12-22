using CodePlusBlog.IService;
using CodePlusBlog.Model;

namespace CodePlusBlog.Service
{
    public class FileService: IFileService
    {
        private readonly IWebHostEnvironment _environment;
        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string SaveFile(string path, List<Files> fileDetail, IFormFileCollection files, string oldFileName = null)
        {
            string currentPath = null;
            foreach (var file in files)
            {
                if (!string.IsNullOrEmpty(file.Name))
                {
                    var currentFile = fileDetail.FirstOrDefault(x => x.FileName == file.Name);
                    if (currentFile != null)
                    {
                        if (!string.IsNullOrEmpty(currentFile.FileNewName))
                            currentFile.FileName = currentFile.FileNewName;

                        var filePath = Path.Combine(_environment.WebRootPath, path);
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        currentFile.FileExtension = file.FileName.Substring(file.FileName.IndexOf('.') + 1);
                          if (!currentFile.FileName.Contains("."))
                            currentFile.FileName = currentFile.FileName + "." + currentFile.FileExtension;

                        if (!string.IsNullOrEmpty(oldFileName))
                        {
                            var oldPath = Path.Combine(filePath, oldFileName);
                            if (File.Exists(oldPath))
                                File.Delete(oldPath);
                        }

                        currentPath = Path.Combine(filePath, currentFile.FileName);
                        using (var stream = new FileStream(currentPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        return currentPath;

                    }
                }
            }
            return currentPath;
        }

        public string SaveTextFile(string path, string body, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, path);
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                string filename = fileName + ".txt";
                var filepath = Path.Combine(filePath, filename);
                if (File.Exists(filepath))
                    File.Delete(filepath);

                var txt = new StreamWriter(filepath);
                txt.Write(body);
                txt.Close();
                return filepath;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
