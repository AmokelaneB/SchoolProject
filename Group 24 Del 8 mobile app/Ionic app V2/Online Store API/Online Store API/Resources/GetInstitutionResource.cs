using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Store_API.Resources
{

    public class GetInstitutionResource
    {
        public int InstitutionID { get; set; }
        public string InstitutionName { get; set; }
        public string Address { get; set; }

        public virtual List<GetFacultyResource> Faculties { get; set; }
    }

   
    public class GetFacultyResource
    {
        public int FacultyID { get; set; }
        
        public string Description { get; set; }

    }

    public class GetIDResource
    {
        public int InstitutionID { get; set; }
        public int FacultyID { get; set; }
        //public int ColourID { get; set; }
    }
}