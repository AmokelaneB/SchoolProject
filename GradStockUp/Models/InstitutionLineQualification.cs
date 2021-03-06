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
    
    public partial class InstitutionLineQualification
    {
        public int StockTypeID { get; set; }
        public int EstablishmentID { get; set; }
        public int InstitutionID { get; set; }
        public int QualificationID { get; set; }
    
        public virtual Establishment Establishment { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual Qualification Qualification { get; set; }
        public virtual StockType StockType { get; set; }
    }
}
