using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradStockUp.Models
{
    public class SupplierOrderDetailsModel
    {
        [Required]
        public string StockType { get; set; }
        [Required]

        public string Colour { get; set; }
        [Required]


        public string Size { get; set; }
        [Required]

        public int Quantity { get; set; }
        [Required]

        public string SupplierName { get; set; }
    }
}