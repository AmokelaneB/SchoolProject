//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GradStockUp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseOrderLine
    {
        public string PurchaseOrderID { get; set; }
        public int StockID { get; set; }
        public int PriceID { get; set; }
    
        public virtual Price Price { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Stock Stock { get; set; }
    }
}
