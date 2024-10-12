using DataAcess.Interfaces;

namespace DataAcess.Services
{
    public class ImageWorker : IImageWorker
    {
        public string ImageSave(string url)
        {
            string imageName = Guid.NewGuid().ToString() + ".webp";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] imageBytes = response.Content.ReadAsByteArrayAsync().Result;
                        var dir = Path.Combine(Directory.GetCurrentDirectory(), "images");
                        var path = Path.Combine(dir, imageName);
                        File.WriteAllBytes(path, imageBytes);

                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve image. Status code: {response.StatusCode}");
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return string.Empty;
            }
            return imageName;
        }
    }
}