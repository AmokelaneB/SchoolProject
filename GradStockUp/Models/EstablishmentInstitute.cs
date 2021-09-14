using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;

namespace GradStockUp.Models
{
    public class EstablishmentInstitute
    {
        public int EstablishmentID { get; set; }
        public int InstitutionID { get; set; }

        public virtual Establishment Establishment { get; set; }
        public virtual Institution Institution { get; set; }


    }
}