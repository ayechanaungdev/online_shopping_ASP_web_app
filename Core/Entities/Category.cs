using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Core.Entities.Attributes.CustomAttributes;

namespace Core.Entities
{
    [Table("Category")]
    public class Category : Entity
    {
        [Required]
        public string Name { get; set; }
        [SkipProperty]
        public ICollection<Product> Products { get; set; }
    }
}
