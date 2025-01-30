using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAcess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;

namespace DataAcess.Services
{
    public class ImageWorker : IImageWorker
    {
        private readonly Cloudinary _cloudinary;

        public ImageWorker(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public async Task<ImageUploadResult> ImageSave(string url)
        {
            string imageName = Guid.NewGuid().ToString() + ".webp";
           
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = response.Content.ReadAsByteArrayAsync().Result;


                    var dir = Path.Combine(Directory.GetCurrentDirectory(), "images");
                    var path = Path.Combine(dir, imageName);

                    Directory.CreateDirectory(dir);
                    File.WriteAllBytes(path, imageBytes);


                    using (var fileStream = new FileStream(path, FileMode.Open))
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(path, fileStream),
                            Folder = "images"
                        };
                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);



                        File.Delete(path);
                        return uploadResult;


                    }
                }
                else
                {
                    return null;
                }
        }

    }


        public async Task<ImageUploadResult> ImageSave(IFormFile file)
        {
          
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = "images" 
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);


                return uploadResult;
                
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                var deletionParams = new DeletionParams(publicId);
                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

                if (deletionResult.Result == "ok")
                {
                    Console.WriteLine("Image deleted successfully from Cloudinary.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Failed to delete image from Cloudinary.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting image: {ex.Message}");
                return false;
            }
        }
    }
}
