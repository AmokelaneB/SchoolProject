using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Online_Store_API.Models;

namespace Online_Store_API.VM
{
    public class Register
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string CustAddress { get; set; }
        public string IDnumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string NextoFKin { get; set; }
        public string kinPhone { get; set; }
        public string Organization { get; set; }
        public string userPassword { get; set; }
        
    }
}