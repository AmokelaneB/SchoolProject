using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GradStockUp.Models
{
    public class InstitutionFacultyLine
    {
        public int FacultyID { get; set; }
        public int InstitutionID { get; set; }

        public virtual Faculty Faculty { get; set; }
        public virtual Institution Institution { get; set; }
    }
}