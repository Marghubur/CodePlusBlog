using CloudinaryDotNet.Actions;

namespace CodePlusBlog.IService
{
    public interface ICloudinaryImageService
    {
        ImageUploadResult UploadFile(IFormFileCollection imageFile);
        Task<string> GetTxtFileAsync(string publicId);
        Task DeleteFile(string publicId);
        ImageUploadResult UploadFile(ImageUploadParams imageUploadParams);
    }
}
