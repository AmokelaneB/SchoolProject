using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Store_API.VM
{
    public class Order
    {
        public int CustomerRentalID { get; set; }
        public int PurchaseOrderID { get; set; }
        public int CustomerID { get; set; }
        public int OrderStatusID { get; set; }
        public int DeliveryID { get; set; }
        public int PaymentMethodID { get; set; }
        public DateTime OrderDateTime { get; set; }
        public int AmountPaid { get; set; }
        public int StockID { get; set; }
        public int PriceID { get; set; }
        public int Quantity { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string CustAddress { get; set; }
        public string PhoneNumber { get; set; }

    }
}