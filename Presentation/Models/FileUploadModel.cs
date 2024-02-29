using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Presentation.Models
{
    public class FileUploadModel
    {
        public string Name { get; set; }

        public IFormFile File { get; set; }
    }
}
