using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CodePlusBlog.IService;
using CodePlusBlog.Model;
using System.Text;

namespace CodePlusBlog.Service
{
    public class CloudinaryImageService : ICloudinaryImageService
    {
        private readonly CloundinarySetting _cloundinarySetting;
        private readonly Cloudinary _cloudinary;
        public CloudinaryImageService(IConfiguration configuration)
        {
            _cloundinarySetting = configuration.GetSection("CloundinarySetting").Get<CloundinarySetting>();
            Account account = new Account(_cloundinarySetting.CloudName, _cloundinarySetting.ApiKey, _cloundinarySetting.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public ImageUploadResult UploadFile(IFormFileCollection imageFile)
        {
            var uploadResult = new ImageUploadResult();
            var currentFile = imageFile.FirstOrDefault();
            Random generator = new Random();
            using (var memoryStream = currentFile.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(currentFile.FileName, memoryStream),
                    PublicId = generator.Next(0, 1000000).ToString("000000")
                };
                uploadResult = _cloudinary.Upload(uploadParams);
            }
            return uploadResult;
        }
        public ImageUploadResult UploadFile(ImageUploadParams imageUploadParams)
        {
            var uploadResult = _cloudinary.Upload(imageUploadParams);
            return uploadResult;
        }

        public void FetchFileFromCloudinary(string link)
        {
            var image = _cloudinary.Api.UrlImgUp.Action("fetch").BuildImageTag(link);
        }

        public async Task DeleteFile(string publicId)
        {
            var deleteParam = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParam);
        }

        public async Task<string> GetTxtFileAsync(string publicId)
        {
            string content = string.Empty;
            var getResourceParams = new GetResourceParams(publicId);

            getResourceParams.ResourceType = ResourceType.Raw; // Or Video, Raw, or Auto

            var result = await _cloudinary.GetResourceAsync(getResourceParams);
            if (result != null && result.Format == "txt")
            {
                using (var streamReader = new StreamReader(result.SecureUrl, Encoding.UTF8))
                {
                    content = streamReader.ReadToEnd();
                    return content;
                }
            }
            return content;
        }
    }
}
