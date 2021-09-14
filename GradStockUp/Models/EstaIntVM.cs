using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRUDs.Models
{
    public class EstaIntVM
    {
        public int InstitutionID { get; set; }
        public string InstitutionName { get; set; }

        public string Address { get; set; }
        public List<EstaInstCheckVM> Establishments { get; set; }
    }
}