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
    
    public partial class AppUser
    {
        public int AppUserID { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string EmplyID { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
