using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GradStockUp.Models
{
    public class EstablishmentStockTypeLine
    {
        public int EstablishmentID { get; set; }
        public int StockTypeID { get; set; }

        public virtual Establishment Establishment { get; set; }
        public virtual StockType StockType { get; set; }
    }
}