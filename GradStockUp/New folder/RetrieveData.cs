using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GradStockUp.Controllers;
using GradStockUp.Models;
namespace GradStockUp
{
    public class RetrieveData
    {

        public string StockBarcode {
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
        public string Department
        {
            get;
            set;
        }
        public string Price
        {
            get;
            set;
        }
        public string location
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }
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
    }
}