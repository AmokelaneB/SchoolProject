using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRUDs.Models
{
    public class EstaInstCheckVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }
}