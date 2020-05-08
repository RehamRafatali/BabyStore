using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BabyStore.Infrastrcture
{
    public  class FileValidation
    {
        private  readonly IHostingEnvironment hostingEnvironment;

        public  FileValidation(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public  bool ValidateFile(IFormFile file)
        {
            string fileExtension= System.IO.Path
                .GetExtension(file.FileName).ToLower();
            string[] allowedFileTypes = { ".gif", ".png", ".jpeg", ".jpg" };
            if ((file.Length > 0 && file.Length < 2097152) &&
            allowedFileTypes.Contains(fileExtension))
            {
                return true;
            }
            return false;
        }

        public  void SaveFileToDisk(IFormFile file)
        {
            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath,
                "ProductImages");
            string filePath = Path.Combine(uploadsFolder, file.FileName);
            file.CopyTo(new FileStream(filePath,FileMode.Create));
        }
    }
}
