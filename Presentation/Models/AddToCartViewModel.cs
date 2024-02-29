using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Presentation.Models
{
    public class AddToCartViewModel
    {
        public int id { get; set; }
        public int quantity { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int UnitPrice { get; set; }

    }
}
