using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace DataAcess.Interfaces
{
    public interface IImageWorker
    {
        Task<ImageUploadResult> ImageSave(string url);
        Task<bool> DeleteImageAsync(string publicId);
        Task<ImageUploadResult> ImageSave(IFormFile file);
    }
}
