using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Core.Entities.Attributes.CustomAttributes;

namespace Core.Entities
{
    [Table("Product")]
    public class Product : Entity
    {
        [DisplayName("Product Name")]
        [Required]
        public string ProductName { get; set; }

        //
        public int Price { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }
        [NotMapped]
        public string ImgName { get; set; }
        public string ImgPath { get; set; }

        public string Information { get; set; }

        public int CategoryId { get; set; }
        [NotMapped]
        [SkipProperty]
        public string CategoryName { get; set; }
        public Category Category { get; set; }
    }
}
