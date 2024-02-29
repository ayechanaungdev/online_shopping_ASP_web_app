using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string  Name { get; set; }
        [Required]
        public string Password { get; set; }

        public string NRC { get; set; }

        // image を保存する
        [NotMapped]
        public IFormFile File { get; set; } // 
        [NotMapped]
        public string ImgName { get; set; } 
        public string ImgPath { get; set; }

    }
}
