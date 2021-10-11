using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Store_API.Resources
{

    public class GetEstablishmentResource
    {
        public int EstablishmentID { get; set; }
        public string EstablishmentName { get; set; }

        public virtual List<GetStockTypeResource> StockTypes { get; set; }
        public virtual List<GetPriceResource> Prices { get; set; }
        public virtual List<GetStockDescriptionResource> StockDescriptions { get; set; }

        public GetEstablishmentResource()
        {
            StockTypes = new List<GetStockTypeResource>();
            Prices = new List<GetPriceResource>();
            StockDescriptions = new List<GetStockDescriptionResource>();

        }
    }

    public class GetStockTypeResource
    {
        public int StockTypeID { get; set; }
        //public int EstablishmentID { get; set; }
       
        public string StockTypeName { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal RentalPrice { get; set; }
        //public int? RetailStockLevel { get; set; }
        //public int? RentalStockLevel { get; set; }

        public virtual List<GetSizeResource> Sizes { get; set; }
        public virtual List<GetPriceResource> Prices { get; set; }
        public virtual List<GetStockDescriptionResource> StockDescriptions { get; set; }

    }

    public class GetSizeResource
    {
        public int SizeID { get; set; }
        //public int StockTypeID { get; set; }
        public string SizeName { get; set; }
        public IEnumerable<GetPriceResource> Prices { get; internal set; }
    }
}
public class GetPriceResource
{
    public int PriceID { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal RentalPrice { get; set; }
    public IEnumerable<GetStockDescriptionResource> StockDescription { get; internal set; }
}

public class GetStockDescriptionResource
{
    public int StockDescriptionID { get; set; }
    public int SizeID { get; set; }
    public int StockTypeID { get; set; }
    public int ColourID { get; set; }
    public int RentalStockLevel { get; set; }
    public int RetailStockLevel { get; set; }


}