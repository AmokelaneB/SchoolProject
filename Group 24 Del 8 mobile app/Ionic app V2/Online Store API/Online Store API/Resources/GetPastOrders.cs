using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Store_API.Resources
{
    public class GetPastOrders
    {
        public string OrderItem { get; set; }
        public string OrderType { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public string Size { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? OrderStatusID { get; set; }
        public int CustomerID { get; set; }
    }
}