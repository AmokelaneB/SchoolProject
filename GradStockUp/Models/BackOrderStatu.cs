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
    
    public partial class BackOrderStatu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BackOrderStatu()
        {
            this.BackOrders = new HashSet<BackOrder>();
        }
    
        public int BackOrderStatusID { get; set; }
        public string BackOrderDescription { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BackOrder> BackOrders { get; set; }
    }
}
