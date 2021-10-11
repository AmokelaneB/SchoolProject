using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Store_API.Resources
{
    public class OrderResource
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public int CardTotal { get; set; }

        public virtual List<OrderItemResource> OrderItems { get; set; }
        public OrderResource()
        {
            OrderItems = new List<OrderItemResource>();
        }

    }

    public class OrderItemResource
    {
        public string item { get; set; }
        public string ordertype { get; set; }
        public int price { get; set; }
        public string size { get; set; }
        public int quantity { get; set; }
    }
   
}