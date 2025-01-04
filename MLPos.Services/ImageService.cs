using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using System.IO;

namespace MLPos.Services;

public class ImageService : IImageService
{
    public string SaveImage(Image image)
    {
        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        string savePath = Path.Combine(image.Path, fileName);

        using (var stream = new FileStream(savePath, FileMode.Create))
        {
            image.ImageStream.CopyTo(stream);
        }

        return fileName;
    }
}