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
    
    public partial class ReturnFromLaundry
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReturnFromLaundry()
        {
            this.ReturnLines = new HashSet<ReturnLine>();
        }
    
        public int ReturnID { get; set; }
        public System.DateTime LaundryReturnDate { get; set; }
        public int Quantity { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReturnLine> ReturnLines { get; set; }
    }
}
