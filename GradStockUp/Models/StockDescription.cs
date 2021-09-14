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
    using System.ComponentModel.DataAnnotations;

    public partial class StockDescription
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockDescription()
        {
            this.BackOrderLines = new HashSet<BackOrderLine>();
            this.Stocks = new HashSet<Stock>();
            this.SupplierOrderLines = new HashSet<SupplierOrderLine>();
        }
    
        public int StockDescriptionID { get; set; }
        public int SizeID { get; set; }
        public int StockTypeID { get; set; }
        public int ColourID { get; set; }
        [Display(Name = "Rental Stock Level")]
        public int RENTALSTOCKLEVEL { get; set; }
        [Display(Name = "Retail Stock Level")]
        public int RETAILSTOCKLEVEL { get; set; }
        [Display(Name = "Retail Threshhold ")]
        public int RETAILTHRESHOLD { get; set; }
        [Display(Name = "Rental Threshold")]
        public int RENTALTHRESHOLD { get; set; }
        [Display(Name = "Flag")]
        public bool FLAG { get; set; }
      
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BackOrderLine> BackOrderLines { get; set; }
        public virtual Colour Colour { get; set; }
        public virtual Size Size { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stock> Stocks { get; set; }
        public virtual StockType StockType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SupplierOrderLine> SupplierOrderLines { get; set; }
    }
}
