using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PhotoSauce.MagicScaler;
using System.Threading.Tasks;

namespace Blog3.Data.FileManager
{
    public class FileManager: IFileManager
    {
        private string _imagePath;

        public FileManager(IConfiguration _config)
        {
            _imagePath = _config["Path:Images"];
        }

        public FileStream ImageStream(string image)
        {
            // throw new NotImplementedException();
            return new FileStream(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);
        }

        public async Task<string> SaveImage(IFormFile image)
        {
            try
            {


                var save_path = Path.Combine(_imagePath);
                if (!Directory.Exists(save_path))
                {
                    Directory.CreateDirectory(save_path);
                }
                //this is to get the last bit whther it is jpeg or png or whatever so it takes the substring from the last index of . to the end
                var mine = image.FileName.Substring(image.FileName.LastIndexOf('.'));
                var fileName = $"img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{mine}";


                using (var fileStream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                    MagicImageProcessor.ProcessImage(image.OpenReadStream(), fileStream, ImageOptions());
                }
                return fileName;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }
            
        }

        private ProcessImageSettings ImageOptions() => new ProcessImageSettings
        {
            Width = 800,
            Height = 500,
            ResizeMode = CropScaleMode.Crop,

            SaveFormat = FileFormat.Jpeg,
            JpegQuality = 100,
            JpegSubsampleMode = ChromaSubsampleMode.Subsample420
        };
    }
}
