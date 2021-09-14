using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GradStockUp.View_Models
{
    public class OrderDescriptions
    {
        public string StockType
        {
            get;
            set;
        }

        public string DescriptionID
        {
            get;
            set;
        }
        public string StockSize
        {
            get;
            set;
        }
        public string StockColour
        {
            get;
            set;
        }
        public int Quantity
        {
            get;
            set;
        }
    }
}