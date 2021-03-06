//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Online_Store_API.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseOrder()
        {
            this.PurchaseOrderLines = new HashSet<PurchaseOrderLine>();
        }
    
        public string PurchaseOrderID { get; set; }
        public int CustomerID { get; set; }
        public int OrderStatusID { get; set; }
        public int PaymentMethodID { get; set; }
        public Nullable<int> DeliveryID { get; set; }
        public System.DateTime OrderDateTime { get; set; }
        public decimal AmountPaid { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Delivery Delivery { get; set; }
        public virtual OrderStatu OrderStatu { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    }
}
