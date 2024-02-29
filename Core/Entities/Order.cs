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
    [Table("Order")]
    public class Order : Entity
    {
        [Required]
        public string CustName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        public int TotalPrice { get; set; }
  
        public string IsDeliver { get; set; }

        [SkipProperty]
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
