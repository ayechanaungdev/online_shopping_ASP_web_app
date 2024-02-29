using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Core.Entities.Attributes.CustomAttributes;

namespace Core.Entities
{
    [Table("OrderDetails")]
    public class OrderDetails:Entity
    {
        [Required]
        public int Qty { get; set; }

        public double QtyPrice { get; set; }

        public string Status { get; set; }

        public int ProductId { get; set; }
        public int OrderId { get; set; }

        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
