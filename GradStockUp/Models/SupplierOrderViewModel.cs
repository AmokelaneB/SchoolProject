using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradStockUp.Models
{
    public class SupplierOrderViewModel
    {
        [Required]
        public string StockType { get; set; }
        [Required]
        public string Colour { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public string SupplierName { get; set; }
        [Required]
        public string Quantity { get; set; }
        [Required]
        public List<SupplierOrderDetailsModel> ListofSupplierOrderDetailModel { get; set; }
    }
}