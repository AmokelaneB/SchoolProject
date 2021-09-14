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
    
    public partial class StockReturn
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockReturn()
        {
            this.StockReturnLines = new HashSet<StockReturnLine>();
        }
    
        public int StockReturnID { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public string CustomerRentalID { get; set; }
    
        public virtual CustomerRental CustomerRental { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockReturnLine> StockReturnLines { get; set; }
    }
}