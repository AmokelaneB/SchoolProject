using GradStockUp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradStockUp.View_Models
{
    public class OrderVM
    {
        public int OnlineOrderID { get; set; }
        public string OrderItem { get; set; }
        public string OrderType { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Size { get; set; }
        public int CustomerID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public System.DateTime OrderDate { get; set; }
        public Nullable<int> OrderStatusID { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual OrderStatu OrderStatu { get; set; }
    }
}