using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ligaNos.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> UpLoadImageAsync(IFormFile imageFile, string folder)
        {
            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.jpg";

            string path = System.IO.Path.Combine(
                Directory.GetCurrentDirectory(),
                $"wwwroot\\pictures\\{folder}",
                file);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"~/pictures/{folder}/{file}";
        }


    }
}
