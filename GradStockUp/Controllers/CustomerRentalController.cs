using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;
using Newtonsoft.Json;
using  iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using GradStockUp.Reporting;

namespace GradStockUp.Controllers
{
    public class CustomerRentalController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();
        string PriceItem;


        // GET: CustomerRental
        public ActionResult Index()
        {
            var customerRentals = db.CustomerRentals.Include(c => c.Customer).Include(c => c.Delivery).Include(c => c.OrderStatu).Include(c => c.PaymentMethod);
            return View(customerRentals.ToList());
        }

        public ActionResult ReceiveCustomerOrder()
        {
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/ReturnedItems.json"), string.Empty);
            return View();

        }

      /* Receive customer Order */
        public ActionResult getState()
        {
            var Stas= db.StockStates.Select(x => new {
                StID = x.StockStateID,
                desc = x.STATE        
            }).ToList();

            return Json(Stas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReserveLater()
        {
            var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/ReturnedItems.json"));
            dynamic Darray = JsonConvert.DeserializeObject(Description);
            List<ReturnedItems> rd = new List<ReturnedItems>();

            foreach (var item in Darray)
            {
                string barc = item.StockBarcode;
                var res = db.Stocks.SingleOrDefault(b => b.StockBarcode == barc);
                if (res != null)
                {

                    if (item.Match == "Matched")
                    {
                        int stateId = Convert.ToInt32(item.State);
                        res.StockStateID = stateId;
                        res.StockStatusID = 1;
                        res.LocationID = 4;
                        int StockDescriptID = res.StockDescriptionID;
                        int Depart = res.DepartmentID;
                        db.SaveChanges();
                        if (stateId == 1)
                            UpdateStockDescript(StockDescriptID);
                        return Json("Success", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json("failed", JsonRequestBehavior.AllowGet);

        }
       
        public ActionResult CaptureItem(string barcode)
        {
            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barcode);
            List<RetrieveData> passData = new List<RetrieveData>();
            RetrieveData d = new RetrieveData();
            if (result != null)
            {
                var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/ReturnedItems.json"));
                dynamic Darray = JsonConvert.DeserializeObject(Description);
                Boolean flag = false;
                foreach (var item in Darray)
                {
                    if (item.StockBarcode == barcode)
                    {
                        flag = true;

                        d.StockBarcode = item.StockBarcode;
                        d.StockColour = item.StockColour;
                        d.StockSize = item.StockSize;
                        d.StockType = item.StockType;
                        break;
                    }
                }

                if (flag)
                {
                    passData.Add(d);
                    return Json((from stck in passData
                                 select new
                                 {
                                     Bar = stck.StockBarcode,
                                     clr = stck.StockColour,
                                     Siz = stck.StockSize,
                                     Stype = stck.StockType

                                 }).ToList(), JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("failed", JsonRequestBehavior.AllowGet);

            }
            return Json("NoItem", JsonRequestBehavior.AllowGet);

        }
        public ActionResult UpdateReceivedStock(string state, string Bar)
        {
            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == Bar);
            List<RetrieveData> passData = new List<RetrieveData>();
            RetrieveData d = new RetrieveData();
            if (result != null)
            {
                var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/ReturnedItems.json"));
                dynamic Darray = JsonConvert.DeserializeObject(Description);
                List<ReturnedItems> rd = new List<ReturnedItems>();
             
                foreach (var item in Darray)
                {
                    if (item.StockBarcode == Bar)
                    {
                        ReturnedItems data = new ReturnedItems();
                        data.DescriptionID = item.DescriptionID.ToString();
                        data.StockBarcode = item.StockBarcode;
                        data.StockColour = item.StockColour;
                        data.StockType = item.StockType;
                        data.Status = item.Status;
                        data.StockSize = item.StockSize;
                        data.Department = item.Department;
                        data.location = item.location;
                        data.Price = item.Price;
                        data.State = state;
                        data.Match = "Matched";
                        rd.Add(data);
                    }
                    else
                    {
                        ReturnedItems data = new ReturnedItems();
                        data.DescriptionID = item.DescriptionID.ToString();
                        data.StockBarcode = item.StockBarcode;
                        data.StockColour = item.StockColour;
                        data.StockType = item.StockType;
                        data.Status = item.Status;
                        data.StockSize = item.StockSize;
                        data.Department = item.Department;
                        data.location = item.location;
                        data.Price = item.Price;
                        data.State = item.State;
                        data.Match = item.Match;
                        rd.Add(data);
                    }

                }

                string json = JsonConvert.SerializeObject(rd.ToArray());
                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/ReturnedItems.json"), json);
                return Json("ItemCaptured", JsonRequestBehavior.AllowGet);
            }
            return Json("Failed", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProceedOutStanding()
        {
            var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/ReturnedItems.json"));
            dynamic Darray = JsonConvert.DeserializeObject(Description);
            List<ReturnedItems> rd = new List<ReturnedItems>();

            foreach (var item in Darray)
            {
                string barc = item.StockBarcode;
                var res = db.Stocks.SingleOrDefault(b => b.StockBarcode == barc);
                if(res != null)
                {

                    if (item.Match == "Matched")
                    {
                        int stateId = Convert.ToInt32(item.State);
                        res.StockStateID = stateId;
                        res.StockStatusID = 1;
                        int StockDescriptID = res.StockDescriptionID;
                        int Depart = res.DepartmentID;
                        db.SaveChanges();
                        if(stateId == 1)
                        UpdateStockDescript(StockDescriptID);
                        return Json("Success", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json("failed", JsonRequestBehavior.AllowGet);
        }

        public void UpdateStockDescript(int UpdateStockDescript)
        {
            var res = db.StockDescriptions.FirstOrDefault(b => b.StockDescriptionID == UpdateStockDescript);
            if (res != null)
            {
                res.RENTALSTOCKLEVEL += 1;
            }
        
        }
        public ActionResult getBanks()
        {

            return Json(db.Banks.Select(x =>
           new {
               BnkID = x.BankID,
               BnkName = x.BankName,
               BCode =x.BranchCode
           }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult getBranchCode(string BankName)
        {
            int Bid = Convert.ToInt32(BankName);
            var res = db.Banks.FirstOrDefault(b => b.BankID == Bid);
            if(res != null)
            {
                return Json(res.BranchCode, JsonRequestBehavior.AllowGet);   

            }
            return Json("", JsonRequestBehavior.AllowGet);
          
        }
        public ActionResult getCustomerOrder(string customerRentalID)
        {
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/ReturnedItems.json"), string.Empty);
            string RentalID = customerRentalID;
            var result = db.CustomerRentals.SingleOrDefault(b => b.CustomerRentalID == RentalID);
            Boolean flag = false;
            if (result != null)
            {

                if (result.OrderStatusID == 3)
                    flag = true;
                List<Customer> customers = new List<Customer>();
                var CustomerResult = db.Customers.SingleOrDefault(b => b.CustomerID == result.CustomerID);
                customers.Add(CustomerResult);

                List<int> StockIDs = new List<int>();
                List<PriceItem> StockPrice = new List<PriceItem>();
                var custoRent = db.CustomerRentalLines.FirstOrDefault(b => b.CustomerRentalID == customerRentalID);

                var customerRentals = db.CustomerRentalLines.Select(x => new {
                    CustRentID = x.CustomerRentalID,
                    StckID = x.StockID,
                    price =x.PriceID
                }).ToList();
                int count = 0;
                string bber ="";
                foreach (var item in customerRentals)
                {
                    if (item.CustRentID == RentalID)
                    {
                        bber =  RentalID;
                        PriceItem priceItem = new PriceItem();
                        int Pid = item.price;
                        var res = db.Prices.Find(Pid);
                        priceItem.Price = Convert.ToString(Math.Round(res.RentalPrice, 2));
                        priceItem.StockID = item.StckID;
                        StockPrice.Add(priceItem);
                        StockIDs.Add(item.StckID);
                        count++;
                      
                    }
                }
                string v = bber;
                List<ReturnedItems> rd = new List<ReturnedItems>();

                foreach (var Obj in StockPrice)
                {
                    ReturnedItems data = new ReturnedItems();
                    int IDs = Obj.StockID;
                    var res = db.Stocks.SingleOrDefault(b => b.StockID == IDs);
                    if (res != null)
                    {

                        data.DescriptionID = res.StockDescriptionID.ToString();
                        data.StockBarcode = res.StockBarcode;
                        data.StockColour = res.StockDescription.Colour.ColourName;
                        data.StockType = res.StockDescription.StockType.DESCRIPTION;
                        data.Status = res.Status.StatusDescription;
                        data.StockSize = res.StockDescription.Size.SizeDescription;
                        data.Department = res.Department.DESCRIPTION;
                        data.location = res.Location.DESCRIPTION;
                        data.Price = Obj.Price;

                        if (res.Status.StatusDescription == "Available")
                        {
                            data.Match = "Matched";
                            data.State =Convert.ToString( res.StockState.STATE);

                        }
                        else
                        {
                            data.Match = "Not Returned";
                            data.State = "NotSet";
                        }
                        rd.Add(data);
                    }


                }
                string json = JsonConvert.SerializeObject(rd.ToArray());

                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/ReturnedItems.json"), json);

                var cb = from b in customers
                         select new
                         {
                             CustName = b.CustomerName + b.CustomerSurname,
                             OrderDate = (result.OrderDateTime).ToString("dd MMMM yyyy"),
                             Items = count,
                             RentId = RentalID,
                             OrderProcess = flag
                         };
                return Json(cb.ToList(), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult OrderSummary()
        {
            /* generate Total*/
            var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/ReturnedItems.json"));
            dynamic Darray = JsonConvert.DeserializeObject(Description);
            double TotalPrice = 0;
            List<ReturnedItems> rd = new List<ReturnedItems>();
           
            foreach (var item in Darray)
            {
                TotalPrice += Convert.ToDouble(item.Price); 
            }
                    /*==================*/
                    return null;
        }


        public ActionResult SubmitReceivedItems(string CustomerRentalID)
        {
            var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/ReturnedItems.json"));
            dynamic Darray = JsonConvert.DeserializeObject(Description);
      
            List<ReturnedItems> rd = new List<ReturnedItems>();
        
            string customerRe = CustomerRentalID;
            var cus = db.CustomerRentals.FirstOrDefault(b => b.CustomerRentalID == customerRe);
            if (cus != null)
            {
                cus.OrderStatusID = 3;
                db.SaveChanges();

             }
            if (Darray != null)
            {
                foreach (var item in Darray)
                {
                    string barcode = item.StockBarcode;
                    var res = db.Stocks.FirstOrDefault(b => b.StockBarcode == barcode);

                    if (res != null)
                    {

                        if (item.Match == "Matched")
                        {
                            if (res.StockStatusID != 1)
                            {
                                res.StockStatusID = 1;
                                int StateiD = Convert.ToInt32(item.State);
                                res.StockStateID = StateiD;
                                res.LocationID = 4;
                                db.SaveChanges();
                                if (StateiD == 1)
                                    UpdateStockDescript(res.StockDescriptionID);
                            }
                        }
                        else
                        {
                            res.StockStatusID = 4;
                            int StateiD = 2;
                            res.StockStateID = StateiD;
                            db.SaveChanges();
                        }
                    }
                }
                updateStockReturn(CustomerRentalID);

            }       
        
            return Json("Done", JsonRequestBehavior.AllowGet);
        }

        public void updateStockReturn(string CustomerId)
        {
            var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/ReturnedItems.json"));
            dynamic Darray = JsonConvert.DeserializeObject(Description);
            Boolean flag = false;
            if (Darray != null)
            {
                    foreach (var item in Darray)
                     {
                        if (item.Match == "Matched")
                        {
                            flag = true;
                        }

                    }
                if (flag)
                {
                    StockReturn stockReturn = new StockReturn();
                    stockReturn.CustomerRentalID = CustomerId;
                    stockReturn.ReturnDate = DateTime.Now;
                    db.StockReturns.Add(stockReturn);
                    db.SaveChanges();
                    foreach (var item in Darray)
                    {
                        if (item.Match == "Matched")
                        {
                            StockReturnLine returnLine = new StockReturnLine();
                            returnLine.StockReturnID = stockReturn.StockReturnID;
                            string barcode = item.StockBarcode;
                            var res = db.Stocks.FirstOrDefault(b => b.StockBarcode == barcode);
                            if(res != null)
                            {
                                returnLine.StockID = res.StockID;
                                returnLine.StockStateID = res.StockStateID;
                                db.StockReturnLines.Add(returnLine);
                                db.SaveChanges();
                            }
                                

                        }

                    }

                }
          
            }
      
        }
        /* end of Receive Customer Order*/

        /* Bulk Transaction Order*/

        public ActionResult BulkTransaction()
        {
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/BulkTrans.json"), string.Empty);
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/BulkOrder.json"), string.Empty);
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/Cart.json"), string.Empty);
            return View();
        }
        public ActionResult ScanItem(string barcode)
        {
            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barcode);
            List<RetrieveData> passData = new List<RetrieveData>();
            RetrieveData d = new RetrieveData();
            if (result != null)
            {
                var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/BulkTrans.json"));
                dynamic Darray = JsonConvert.DeserializeObject(Description);
                Boolean flag = false;
                foreach (var item in Darray)
                {
                    if (Convert.ToInt32(item.DescriptionID) == result.StockDescriptionID)
                    {
                        flag = true;

                        d.StockBarcode = result.StockBarcode;
                        d.StockColour = result.StockDescription.Colour.ColourName;
                        d.StockSize = result.StockDescription.Size.SizeDescription;
                        d.StockType = result.StockDescription.StockType.DESCRIPTION;
                        break;
                    }
                }

                if (flag)
                {
                    passData.Add(d);
                    return Json((from stck in passData
                                 select new
                                 {
                                     Bar = stck.StockBarcode,
                                     clr = stck.StockColour,
                                     Siz = stck.StockSize,
                                     Stype = stck.StockType

                                 }).ToList(), JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("failed", JsonRequestBehavior.AllowGet);

            }
            return Json("NoItem", JsonRequestBehavior.AllowGet);

        }


        public void AddToCart()
        { 
        
        
        }
        public ActionResult BulkItem(string Bar)
        {
            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == Bar);
            List<RetrieveData> passData = new List<RetrieveData>();
            RetrieveData d = new RetrieveData();
            Boolean flag = false;
            Boolean RentalFlag = false;
            int RentPrice = 0;
            int RetaiPrice = 0;
            if (result != null)
            {
                var Des = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/BulkTrans.json"));
                dynamic arr = JsonConvert.DeserializeObject(Des);
                var BulkList = JsonConvert.DeserializeObject<List<BulkTrans>>(Des)
               ?? new List<BulkTrans>();

                List<BulkTrans> tempList = new List<BulkTrans>();
                if (arr != null)
                {
                    foreach (var Obj in arr)
                    {
                        if (Obj.DescriptionID == result.StockDescriptionID.ToString())
                        {
                            flag = true;
                            BulkTrans bulk = new BulkTrans();
                            bulk.DescriptionID = Obj.DescriptionID;
                            bulk.Faculty = Obj.Faculty;
                            bulk.StockType = Obj.StockType;
                            bulk.Size = Obj.Size;
                            bulk.Colour = Obj.Colour;
                            bulk.RetailPrice = Obj.RetailPrice;
                            bulk.RentalPrice = Obj.RentalPrice;
                   
                            string Univ = Obj.Insitute;
                            var InstObj = db.Institutions.FirstOrDefault(b => b.InstitutionName == Univ);
                            if (InstObj != null)
                            {
                                int InstituId = InstObj.InstitutionID;
                                string Est = Obj.Establishment;
                                var EstObj = db.Establishments.FirstOrDefault(b => b.EstaName == Est);
                                if (EstObj != null)
                                {
                                    int EstID = EstObj.EstablishmentID;
                                    string StckType = Obj.StockType;
                                    var StockObj = db.StockTypes.FirstOrDefault(b => b.DESCRIPTION == StckType);

                                    if (StockObj != null)
                                    {
                                        int StckTypeID = StockObj.StockTypeID;
                                        var res = db.Prices.FirstOrDefault(b => b.InstitutionID == InstituId && b.EstablishmentID == EstID && b.StockTypeID == StckTypeID);

                                        if (res != null)
                                        {
                                            List<Price> prices = new List<Price>();
                                            prices.Add(res);
                                            int count = 0;
                                            DateTime dateTime = DateTime.Now;
                                            foreach (var B in prices)
                                            {
                                                if (count == 0)
                                                {
                                                    dateTime = B.PriceDate;
                                                    RentPrice = Convert.ToInt32(B.PriceID);
                                                    RetaiPrice = Convert.ToInt32(B.PriceID);
                                                }

                                                count++;

                                                if (DateTime.Compare(dateTime, B.PriceDate) > 0)
                                                {
                                                    RentPrice = Convert.ToInt32(B.PriceID);
                                                    RetaiPrice = Convert.ToInt32(B.PriceID);
                                                }

                                            }

                                        }
                                    }

                                }
                            }
                            /* End of Price Setting"*/
                            if (result.DepartmentID == 1)
                            {
                                if (Obj.RentalQty > 0)
                                    bulk.RentalQty = (Convert.ToInt32(Obj.RentalQty) - 1).ToString();
                                else
                                {
                                    RentalFlag = true;

                                }

                            }
                            else
                            {
                                if (Obj.RetailQty > 0)
                                    bulk.RentalQty = (Convert.ToInt32(Obj.RetailQty) - 1).ToString();
                                else
                                {
                                    RentalFlag = true;

                                }
                            }

                            if (RentalFlag)
                            {
                                bulk.RetailQty = Obj.RetailQty;
                                bulk.RentalQty = Obj.RentalQty;

                            }

                            bulk.Qualification = Obj.Qualification;
                            bulk.Insitute = Obj.Insitute;
                            bulk.Establishment = Obj.Establishment;
                            tempList.Add(bulk);


                        }
                        else
                        {
                            BulkTrans bulk = new BulkTrans();
                            bulk.DescriptionID = Obj.DescriptionID;
                            bulk.Faculty = Obj.Faculty;
                            bulk.StockType = Obj.StockType;
                            bulk.Size = Obj.Size;
                            bulk.Colour = Obj.Colour;
                            bulk.RetailPrice = Obj.RetailPrice;
                            bulk.RentalPrice = Obj.RentalPrice;
                            bulk.RentalQty = Obj.RentalQty;
                            bulk.RetailQty = Obj.RetailQty;
                            bulk.Qualification = Obj.Qualification;
                            bulk.Insitute = Obj.Insitute;
                            bulk.Establishment = Obj.Establishment;
                            tempList.Add(bulk);


                        }
                    }

                    if (flag && !RentalFlag)
                    {
                        var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/BulkOrder.json"));
                        dynamic Darray = JsonConvert.DeserializeObject(Description);
                        var Cart = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/Cart.json"));
                        dynamic CartArr = JsonConvert.DeserializeObject(Cart);
                        List<Descriptions> descriptions = new List<Descriptions>();
                        var rd = JsonConvert.DeserializeObject<List<ReturnedItems>>(Description)
                               ?? new List<ReturnedItems>();
                        var CartList = JsonConvert.DeserializeObject<List<Descriptions>>(Cart)
                               ?? new List<Descriptions>();
                        Boolean f = false;
                        if (Darray != null)
                        {
                            foreach (var item in Darray)
                            {
                                if (item.StockBarcode == Bar)
                                {
                                    f = true;
                                }

                            }

                            if (!f)
                            {
                                ReturnedItems data = new ReturnedItems();
                                data.DescriptionID = result.StockDescriptionID.ToString();
                                data.StockBarcode = result.StockBarcode;
                                data.StockColour = result.StockDescription.Colour.ColourName;
                                data.StockType = result.StockDescription.StockType.DESCRIPTION;
                                data.Status = result.Status.StatusDescription;
                                data.StockSize = result.StockDescription.Size.SizeDescription;
                                data.Department = result.Department.DESCRIPTION;

                                if (data.Department == "Rental")
                                {
                                    data.Price = Convert.ToString(RentPrice);
                                }
                                else
                                    data.Price = Convert.ToString(RetaiPrice);

                                rd.Add(data);
                                string json = JsonConvert.SerializeObject(rd.ToArray());
                                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/BulkOrder.json"), json);
                        
                                Boolean flagged = false;
                                if (CartArr != null)
                                {
                                    foreach (var Obj in CartArr)
                                    {
                                        if (Obj.DescriptionID == data.DescriptionID)
                                        {

                                            if (Obj.Department == data.Department)
                                            {
                                                Descriptions dp = new Descriptions();
                                                dp.DescriptionID = Obj.DescriptionID;
                                                dp.StockColour = Obj.StockColour;
                                                dp.StockSize = Obj.StockSize;
                                                dp.Quantity = Obj.Quantity + 1;
                                                dp.Department = Obj.Department;
                                                dp.StockType = Obj.StockType;
                                                dp.Price = Obj.Price;
                                                dp.TotalPrice = dp.Quantity * dp.Price;
                                                flagged = true;
                                                descriptions.Add(dp);

                                            }
                                            else
                                            {
                                                Descriptions dp = new Descriptions();
                                                dp.DescriptionID = Obj.DescriptionID;
                                                dp.StockColour = Obj.StockColour;
                                                dp.StockSize = Obj.StockSize;
                                                dp.Quantity = Obj.Quantity;
                                                dp.Department = Obj.Department;
                                                dp.TotalPrice = Obj.TotalPrice;
                                                dp.StockType = Obj.StockType;
                                                dp.Price = Obj.Price;
                                                descriptions.Add(dp);
                                                flagged = false;
                                            }
                                              

                                        }
                                        else
                                        {
                                            Descriptions dp = new Descriptions();
                                            dp.DescriptionID = Obj.DescriptionID;
                                            dp.StockColour = Obj.StockColour;
                                            dp.StockSize = Obj.StockSize;
                                            dp.Quantity = Obj.Quantity;
                                            dp.Department = Obj.Department;
                                            dp.TotalPrice = Obj.TotalPrice;
                                            dp.StockType = Obj.StockType;
                                            dp.Price = Obj.Price;
                                            descriptions.Add(dp);

                                        }
                                    }

                                    if (!flagged)
                                    {
                                        Descriptions dp = new Descriptions();
                                        dp.DescriptionID = data.DescriptionID;
                                        dp.StockColour = data.StockColour;
                                        dp.StockSize = data.StockSize;
                                        dp.Quantity = 1;
                                        dp.Department = data.Department;
                                        dp.StockType = data.StockType;
                                        if (dp.Department == "Rental")
                                            dp.Price = Convert.ToDouble(db.Prices.Find(RentPrice).RentalPrice);
                                        else
                                            dp.Price = Convert.ToDouble(db.Prices.Find(RetaiPrice).RetailPrice);

                                        dp.TotalPrice = dp.Quantity * dp.Price;
                                        descriptions.Add(dp);
                                    }
                                    string J = JsonConvert.SerializeObject(descriptions.ToArray());
                                    System.IO.File.WriteAllText(Server.MapPath("~/Scripts/Cart.json"), J);
                                    return Json(descriptions, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                   
                                        Descriptions dp = new Descriptions();
                                        dp.DescriptionID = data.DescriptionID;
                                    dp.StockType = data.StockType;
                                        dp.StockColour = data.StockColour;
                                        dp.StockSize = data.StockSize;
                                        dp.Quantity = 1;
                                        dp.Department = data.Department;
                                    if (dp.Department == "Rental")
                                        dp.Price = Convert.ToDouble(db.Prices.Find(RentPrice).RentalPrice);
                                    else
                                        dp.Price = Convert.ToDouble(db.Prices.Find(RetaiPrice).RetailPrice);

                                    dp.TotalPrice = dp.Quantity * dp.Price;
                                    descriptions.Add(dp);
                                  
                                    string J = JsonConvert.SerializeObject(descriptions.ToArray());
                                    System.IO.File.WriteAllText(Server.MapPath("~/Scripts/Cart.json"), J);
                                    return Json(descriptions, JsonRequestBehavior.AllowGet);

                                }
                            }

                        }
                        else
                        {
                            
                                ReturnedItems data = new ReturnedItems();
                                data.DescriptionID = result.StockDescriptionID.ToString();
                                data.StockBarcode = result.StockBarcode;
                                data.StockColour = result.StockDescription.Colour.ColourName;
                                data.StockType = result.StockDescription.StockType.DESCRIPTION;
                                data.Status = result.Status.StatusDescription;
                                data.StockSize = result.StockDescription.Size.SizeDescription;
                                data.Department = result.Department.DESCRIPTION;

                                if (data.Department == "Rental")
                                {
                                    data.Price = Convert.ToString(RentPrice);
                                }
                                else
                                    data.Price = Convert.ToString(RetaiPrice);

                                rd.Add(data);
                                string json = JsonConvert.SerializeObject(rd.ToArray());
                                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/BulkOrder.json"), json);

                            Descriptions dp = new Descriptions();
                            dp.DescriptionID = data.DescriptionID;
                            dp.StockColour = data.StockColour;
                            dp.StockSize = data.StockSize;
                            dp.StockType = data.StockType;
                            dp.Quantity = 1;
                            dp.Department = data.Department;
                            if (dp.Department == "Rental")
                                dp.Price =Convert.ToDouble( db.Prices.Find(RentPrice).RentalPrice);
                            else
                                dp.Price = Convert.ToDouble(db.Prices.Find(RetaiPrice).RetailPrice);

                            dp.TotalPrice = dp.Quantity * dp.Price;
                            descriptions.Add(dp);

                            string J = JsonConvert.SerializeObject(descriptions.ToArray());
                            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/Cart.json"), J);
                            return Json(descriptions, JsonRequestBehavior.AllowGet);

                        }
                       

                    }

                }
         
                return Json("ItemCaptured", JsonRequestBehavior.AllowGet);
            }
            return Json("Failed", JsonRequestBehavior.AllowGet);
        }
        public ActionResult getCustomer(string CustomerID)
        {
            
            var res = db.Customers.FirstOrDefault(b => b.IDnumber == CustomerID);
            List<Customer> cust = new  List<Customer>();
            if (res != null)
            {
                cust.Add(res);
                var resul = from Establish in cust
                            select new
                            {
                                CusId = Establish.CustomerID,
                                CustName = Establish.CustomerName + "  " + Establish.CustomerSurname,
                                CustEmail = Establish.Email,
                                CustNum = Establish.PhoneNumber,
                                OrgaID = Establish.OrganizationID

                            };
                return Json(resul.ToList(), JsonRequestBehavior.AllowGet);
            }

            return Json("NotFound", JsonRequestBehavior.AllowGet);
  
        
        }
        public ActionResult getOrganization(string OrganizationID)
        {
            int id = Convert.ToInt32(OrganizationID);


            var OrganizationD = db.getOrganizationByID(id);

            if (OrganizationD != null)
            {
                List<Organization> org = new List<Organization>();

                Organization b = new Organization();
                    var Or= OrganizationD.ToList();
             
                var resul = from Obj in Or
                            select new
                            {
                                OrgName = Obj.Name,
                                OrgAddress =Obj.Address,
                                OrgContact =Obj.Contact,
                                OrgReg = Obj.RegistrationNO

                            };
                return Json(resul.ToList(), JsonRequestBehavior.AllowGet);
            
            }
            return Json("NotFound", JsonRequestBehavior.AllowGet);
        
        }

        public ActionResult PlaceBulkOrder(string CustID, string Name, string RentalAmount,  string RetailAmount)
        {
            List<CustomerDetails> customerDetails = new List<CustomerDetails>();
            CustomerDetails details = new CustomerDetails();
            int customeID = Convert.ToInt32(CustID);
            Double Cost = double.Parse(RentalAmount, System.Globalization.CultureInfo.InvariantCulture);
            Decimal TotalCost = Convert.ToDecimal(Cost);
 
            string CustRent;
            string CustPurch;
            Boolean flag = false;
            Boolean rentalFlag = false;
            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/BulkOrder.json"));
            dynamic array = JsonConvert.DeserializeObject(jsonData);
            if (array != null)
            {
                foreach (var item in array)
                {
                    if (item.Department == "Rental")
                    {
                        rentalFlag = true;
                    }
                    if (item.Department == "Retail")
                    {
                        flag = true;
                    }
                }
            }

            if (rentalFlag)
            {

                string customerRentalID = GenerateCustomerRentalID();
                details.RentalID = customerRentalID;
                CustRent = customerRentalID;
                CustomerRental customer = new CustomerRental();
                customer.CustomerRentalID = customerRentalID;
                customer.CustomerID = customeID;
                customer.OrderStatusID = 1;
                customer.PaymentMethodID = 1;
                customer.OrderDateTime = DateTime.Now;
                customer.AmountPaid = TotalCost;

                db.CustomerRentals.Add(customer);
                db.SaveChanges();

                foreach (var item in array)
                {
                    if (item.Department == "Rental")
                    {
                        CustomerRentalLine customerRental = new CustomerRentalLine();
                        customerRental.CustomerRentalID = customer.CustomerRentalID;
                        string barcode = item.StockBarcode;
                        var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barcode);
                        customerRental.StockID = result.StockID;
                        int StID = result.StockDescription.StockTypeID;
                        int PrcID = Convert.ToInt32(item.Price);
                        customerRental.PriceID = PrcID;
                        db.CustomerRentalLines.Add(customerRental);
                        db.SaveChanges();
                        UpdateItemStatus(barcode, 2);
                        UpdateRentalDescription(result.StockDescriptionID, 1);
                    }


                }
            }


            if (flag)
            {
                string PurchID = GenerateCustomerRentalID();
                CustPurch = PurchID;
                details.PurchaseID = PurchID;
                PurchaseOrder purchase = new PurchaseOrder();
                purchase.PurchaseOrderID = PurchID;
                purchase.CustomerID = customeID;
                purchase.OrderStatusID = 1;
                purchase.PaymentMethodID = 1;
                TotalCost = Convert.ToDecimal(double.Parse(RetailAmount, System.Globalization.CultureInfo.InvariantCulture));
                purchase.OrderDateTime = DateTime.Now;
                purchase.AmountPaid = TotalCost;
                db.PurchaseOrders.Add(purchase);
                db.SaveChanges();

                foreach (var item in array)
                {
                    if (item.Department == "Retail")
                    {
                        PurchaseOrderLine customerRental = new PurchaseOrderLine();
                        customerRental.PurchaseOrderID = purchase.PurchaseOrderID;
                        string barcode = item.StockBarcode;
                        var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barcode);
                        customerRental.StockID = result.StockID;
                        int StID = result.StockDescription.StockTypeID;
                        int PrcID = Convert.ToInt32(item.Price);
                     
                        customerRental.PriceID = PrcID;
                        db.PurchaseOrderLines.Add(customerRental);
                        db.SaveChanges();
                        UpdateItemStatus(barcode, 3);
                        UpdateRentalDescription(result.StockDescriptionID, 2);
                    }
                }
            }

            if (flag || rentalFlag)
            {
                details.CustomerName = Name;
                details.EmployeeName = "Mr Mahlangu ";
                customerDetails.Add(details);
                return Json(PrepareReport(customerDetails), JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        /*=================End of Transaction=============*/

        public ActionResult SaveOrg(string OrgName, string OrgAddres, string Contact, string Reg, string CustomerID)
        {

            var OrgDb = db.Organizations.SingleOrDefault(b => b.RegistrationNO == Reg && b.Name == OrgName);
            if (OrgDb == null)
            {
                Organization organization = new Organization()
                {
                    RegistrationNO = Reg,
                    Address = OrgAddres,
                    Contact = Contact,
                    Name = OrgName
                };
                db.Organizations.Add(organization);
                db.SaveChanges();
                int CustID = Convert.ToInt32(CustomerID);

                var res = db.Customers.Find(CustID);
                if (res != null)
                {
                    res.OrganizationID = organization.OrganizationID;
                    db.SaveChanges();
                    @TempData["SuccessMessage"] = "Organization has Been Saved";
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                @TempData["ErrorMessage"] = "Customer Not linked to Organization";
                return Json("failed", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int CustID = Convert.ToInt32(CustomerID);
                 
                var res = db.Customers.Find(CustID);
                if (res != null)
                {
                    res.OrganizationID = OrgDb.OrganizationID;
                    db.SaveChanges();
                    @TempData["SuccessMessage"] = "Organization has Been Saved";
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                @TempData["ErrorMessage"] = "Customer Not linked to Organization";
                return Json("failed", JsonRequestBehavior.AllowGet);

            }

        }

        public void DeleteFromStockList(string BarCode)
        {
            var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/BulkOrder.json"));
            dynamic Darray = JsonConvert.DeserializeObject(Description);
            var Cart = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/Cart.json"));
            dynamic CartArr = JsonConvert.DeserializeObject(Cart);
            List<Descriptions> descriptions = new List<Descriptions>();
            var rd = JsonConvert.DeserializeObject<List<ReturnedItems>>(Description)
                   ?? new List<ReturnedItems>();
            List<ReturnedItems> BulkList  = new List<ReturnedItems>();
            var CartList = JsonConvert.DeserializeObject<List<Descriptions>>(Cart)
                   ?? new List<Descriptions>();
            Boolean f = false;
            string DescriptId = "";
            string DepartmentID = "";
            if (Darray != null)
            {
                foreach (var item in Darray)
                {
                    if (item.StockBarcode == BarCode)
                    {
                        f = true;
                        DescriptId = item.DescriptionID;
                        DepartmentID = item.Department;
                    }
                    else
                    {
                        ReturnedItems data = new ReturnedItems();
                        data.DescriptionID = item.DescriptionID;
                        data.StockBarcode = item.StockBarcode;
                        data.StockColour = item.StockColour;
                        data.StockType = item.StockType;
                        data.Status = item.Status;
                        data.StockSize = item.StockSize;
                        data.Department = item.Department;
                        data.Price = item.Price;
                        BulkList.Add(data);
                    }
                }
                string JsonDescription = JsonConvert.SerializeObject(BulkList.ToArray());
                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/BulkOrder.json"), JsonDescription);

                Boolean flagged = false;
                if (CartArr != null)
                {
                    foreach (var Obj in CartArr)
                    {
                        if (Obj.DescriptionID == DescriptId)
                        {

                            if (Obj.Department == DepartmentID)
                            {
                                Descriptions dp = new Descriptions();
                                dp.DescriptionID = Obj.DescriptionID;
                                dp.StockColour = Obj.StockColour;
                                dp.StockSize = Obj.StockSize;

                                int Qty = Convert.ToInt32(Obj.Quantity)-1;
                                dp.Quantity = Qty;
                                dp.Department = Obj.Department;
                                dp.StockType = Obj.StockType;
                                dp.Price = Obj.Price;
                                dp.TotalPrice = dp.Quantity * dp.Price;
                                flagged = true;
                                if(dp.Quantity > 0)
                                descriptions.Add(dp);
                            }
                            else
                            {
                                Descriptions dp = new Descriptions();
                                dp.DescriptionID = Obj.DescriptionID;
                                dp.StockColour = Obj.StockColour;
                                dp.StockSize = Obj.StockSize;
                                dp.Quantity = Obj.Quantity;
                                dp.Department = Obj.Department;
                                dp.StockType = Obj.StockType;
                                dp.Price = Obj.Price;
                                dp.TotalPrice = dp.Quantity * dp.Price;
                                flagged = true;
                                if (dp.Quantity > 0)
                                    descriptions.Add(dp);
                             

                            }

                        }
                        else
                        {
                            Descriptions dp = new Descriptions();
                            dp.DescriptionID = Obj.DescriptionID;
                            dp.StockColour = Obj.StockColour;
                            dp.StockSize = Obj.StockSize;
                            dp.Quantity = Obj.Quantity;
                            dp.Department = Obj.Department;
                            dp.StockType = Obj.StockType;
                            dp.Price = Obj.Price;
                            dp.TotalPrice = dp.Quantity * dp.Price;
                            flagged = true;
                            descriptions.Add(dp);
                        }
                    }
                    string Jso = JsonConvert.SerializeObject(descriptions.ToArray());
                    System.IO.File.WriteAllText(Server.MapPath("~/Scripts/Cart.json"), Jso);
                }

        }
             
            

        }
        public ActionResult SaveCustomer(string CustID, string CustName, string PhoneNum, string Address, string Email, string NextKinName, string NextKinPhone, string CustSur)
        {


            string CustomerID = CustID;
            Customer customer = new Customer();

            var cust = db.Customers.Where(b => b.IDnumber == CustomerID);
            if (cust != null)
            {
             customer.OrganizationID = null;
            customer.IDnumber = CustomerID;
                customer.CustomerName = CustName;
            customer.CustomerSurname = CustSur;
            customer.Email = Email;
            customer.CustAddress = Address;
            customer.PhoneNumber = PhoneNum;
            customer.NextoFKin = NextKinName;
            customer.kinPhone = NextKinPhone;
         

                db.Customers.Add(customer);
                db.SaveChanges();
                @TempData["SuccessMessage"] = "Customer has been saved succesfully, Please Assign a customer an Organization";
                return Json(CustomerID, JsonRequestBehavior.AllowGet);
            }

            else
            {
                @TempData["ErrorMessage"] = "Customer of ID " + CustomerID + " has been registered on the system";
                return Json("failed", JsonRequestBehavior.AllowGet);
            }
              
           
          
        }

        public ActionResult GetCustomers()
        {

            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/AddListItems.json"), string.Empty);
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/TransactionItems.json"), string.Empty);
            return Json(new
            {
                data = db.Customers.Select(x => new 
                {
                    space = "",
                    CustmerID = x.CustomerID,
                    CustID = x.IDnumber,
                    CustName = x.CustomerName,
                    CustSurname = x.CustomerSurname,
                    CustPhone = x.PhoneNumber,
                    CustEmail = x.Email,
                }).ToList()
            }, JsonRequestBehavior.AllowGet); ;
        }
        public ActionResult getInstitution()
        {

            var Sizes = from Establish in db.Establishments
                        from Institute in Establish.Institutions
                        select new
                        {
                            InstID = Institute.InstitutionID,
                            description = Institute.InstitutionName,
                            EstaID = Establish.EstablishmentID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetQoute(string Quali, string EstablishID, string Fac, string InstituteID, string StockType, string RetailQty, string RentQty, string StockSize)
        {

            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/BulkTrans.json"));
            var BulkList = JsonConvert.DeserializeObject<List<BulkTrans>>(jsonData)
                   ?? new List<BulkTrans>();
            int EstID = Convert.ToInt32(EstablishID);
            int InstituId = Convert.ToInt32(InstituteID);

            int StckType = Convert.ToInt32(StockType);
            int bb = Convert.ToInt32(StockType);
            int Colour = 0;
            int SizeItem = Convert.ToInt32(StockSize);
            var res = db.Prices.SingleOrDefault(b => b.InstitutionID == InstituId && b.EstablishmentID == EstID && b.StockTypeID == StckType);
            string RentalPrice = "";
            string RetailPrice = "";
            int Rent = Convert.ToInt32(RentQty);
            int Retai = Convert.ToInt32(RetailQty);
            if (res != null)
            {
                RentalPrice = Convert.ToString(Math.Round(Convert.ToDecimal(res.RentalPrice) * Rent,2)); ;
                RetailPrice = Convert.ToString(Math.Round(Convert.ToDecimal(res.RetailPrice) * Retai,2));
            }

            string facTit = "";
            if (Fac != "")
            {
                int FacID = Convert.ToInt32(Fac);
                var fa = db.Faculties.Find(FacID);
                facTit = fa.Description;
                  var result = db.InstitutionFaculties.SingleOrDefault(b => b.InstitutionID == InstituId && b.FacultyID == FacID);

                if (getColour(StockType, Quali) == "NoColour")
                {
                    Colour = result.ColourID;

                }
                else
                {
                    Colour = Convert.ToInt32(getColour(StockType, Quali));
                }
            }
            else
            {
                Colour = Convert.ToInt32(Quali);
                facTit = "NoFaculty";
            }
               
         
            var StockDescript = db.StockDescriptions.SingleOrDefault(b => b.StockTypeID == bb && b.SizeID == SizeItem && b.ColourID == Colour);

            if (StockDescript != null)
            {
                BulkTrans bulk = new BulkTrans();
                bulk.DescriptionID = StockDescript.StockDescriptionID.ToString();
               
                var resu = db.Establishments.Find(EstID);
                if(resu != null)
                bulk.Establishment = resu.EstaName;
                var resuInst = db.Institutions.Find(InstituId);
                if(resuInst != null)
                bulk.Insitute = resuInst.InstitutionName;
                int qID = Convert.ToInt32(Quali);
                var Qual = db.Qualifications.Find(qID);
                bulk.Qualification = Qual.QualificationName;


                bulk.Faculty = facTit;
                bulk.StockType = StockDescript.StockType.DESCRIPTION;
                bulk.Size = StockDescript.Size.SizeDescription;
                bulk.Colour = StockDescript.Colour.ColourName;
                bulk.RetailPrice = RetailPrice;
                bulk.RentalPrice = RentalPrice;
                bulk.RentalQty = RentQty;
                bulk.RetailQty = RetailQty;
                List<BulkTrans> b = new List<BulkTrans>();
                b.Add(bulk);
                string JsonDescription = JsonConvert.SerializeObject(b.ToArray());
                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/TemData.json"), JsonDescription);
                var JsonString = JsonConvert.SerializeObject(b.ToArray());
                return Json(b, JsonRequestBehavior.AllowGet);
          
            }

            return Json("failed", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReserveItem()
        {
            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/BulkTrans.json"));
            var BulkList = JsonConvert.DeserializeObject<List<BulkTrans>>(jsonData)
                   ?? new List<BulkTrans>();

            var Temp = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/TemData.json"));
            var TempList = JsonConvert.DeserializeObject<List<BulkTrans>>(Temp)
                   ?? new List<BulkTrans>();
            dynamic Darray = JsonConvert.DeserializeObject(jsonData);
            dynamic arr = JsonConvert.DeserializeObject(Temp);
            Boolean flag = false;
            if (arr != null)
            {
                foreach(var Obj in arr)
                {
                    if (Darray != null)
                    {
                        foreach (var item in Darray)
                        {
                            if (Obj.StockDescriptionID == item.StockDescrptionID)
                            {
                                flag = true;
                                break;

                            }

                        }

                        if (flag)
                        {
                            BulkTrans bulk = new BulkTrans();
                            bulk.DescriptionID = Obj.DescriptionID;
                            bulk.Faculty = Obj.Faculty;
                            bulk.StockType = Obj.StockType;
                            bulk.Size = Obj.Size;
                            bulk.Colour = Obj.Colour;
                            bulk.RetailPrice = Obj.RetailPrice;
                            bulk.RentalPrice = Obj.RentalPrice;
                            bulk.RentalQty = Obj.RentalQty;
                            bulk.RetailQty = Obj.RetailQty;
                            bulk.Qualification = Obj.Qualification;
                            bulk.Insitute = Obj.Insitute;
                            bulk.Establishment = Obj.Establishment;
                            BulkList.Add(bulk);
                            string JsonDescription = JsonConvert.SerializeObject(BulkList.ToArray());
                            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/BulkTrans.json"), JsonDescription);
                            var JsonString = JsonConvert.SerializeObject(BulkList.ToArray());
                            return Json("Done", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        BulkTrans bulk = new BulkTrans();
                        bulk.DescriptionID = Obj.DescriptionID;
                        bulk.Faculty = Obj.Faculty;
                        bulk.StockType = Obj.StockType;
                        bulk.Size = Obj.Size;
                        bulk.Colour = Obj.Colour;
                        bulk.RetailPrice = Obj.RetailPrice;
                        bulk.RentalPrice = Obj.RentalPrice;
                        bulk.RentalQty = Obj.RentalQty;
                        bulk.RetailQty = Obj.RetailQty;
                        bulk.Qualification = Obj.Qualification;
                        bulk.Insitute = Obj.Insitute;
                        bulk.Establishment = Obj.Establishment;
                        BulkList.Add(bulk);
                        string JsonDescription = JsonConvert.SerializeObject(BulkList.ToArray());
                        System.IO.File.WriteAllText(Server.MapPath("~/Scripts/BulkTrans.json"), JsonDescription);
                        var JsonString = JsonConvert.SerializeObject(BulkList.ToArray());
                        return Json("Done", JsonRequestBehavior.AllowGet);
                    }
                  


                }

                

                }

            return Json("Failed", JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPrice(string EstablshID, string InstID, string stckType, string Trans)
        {
            int InstitID = Convert.ToInt32(InstID);
            int EstID = Convert.ToInt32(EstablshID);
            int StckID = Convert.ToInt32(stckType);
            var result = db.Prices.SingleOrDefault(b => b.InstitutionID == InstitID && b.EstablishmentID == EstID && b.StockTypeID == StckID);
            string Price = "R";
            if (result != null)
            {
                if (Convert.ToInt32(Trans) == 2)
                    Price = Convert.ToString(result.RetailPrice);
                else
                    Price = Convert.ToString(result.RentalPrice);
            }
            PriceItem = Price;
            return Json(Price, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getEstablishment()
        {
            return Json(db.Establishments.Select(x => new
            {

                EstaID = x.EstablishmentID,
                Description = x.EstaName,

            }).ToList(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult PrepareReport(List<CustomerDetails> customerDetails)
        {
            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/TransactionItems.json"));
            var employeeList = JsonConvert.DeserializeObject<List<RetrieveData>>(jsonData)
                   ?? new List<RetrieveData>();
            CustomerTransactionReport transferRepo = new CustomerTransactionReport();
            string ImgPath = Server.MapPath("~/Reporting/LetterHead.png");
            byte[] aBytes = transferRepo.PrepareReport(employeeList, ImgPath, customerDetails);


            System.IO.File.WriteAllBytes(Server.MapPath("~/Reporting/CustomerReport.pdf"), aBytes);
            return File(aBytes, "application/pdf");
        }

        public ActionResult getStockType()
        {
            var Sizes = from Establish in db.Establishments
                        from StckType in Establish.StockTypes
                        select new
                        {
                            StckTypeID = StckType.StockTypeID,
                            description = StckType.DESCRIPTION,
                            EstaID = Establish.EstablishmentID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadData()
        {

            var stockDescriptions = db.StockDescriptions.Include(s => s.Colour).Include(s => s.Size.SizeDescription).Include(s => s.StockType.DESCRIPTION);

            var b = (from obj in stockDescriptions select new { ID = obj.StockDescriptionID, ColourName = obj.Colour.ColourName, Size = obj.Size.SizeDescription, StockType = obj.StockType.DESCRIPTION, space = "" });

            return Json(new { data = b.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public string getColour(string stockType, string Qualification)
        {
            int st = Convert.ToInt32(stockType);
            int qu = Convert.ToInt32(Qualification);
            var res = db.QualificationStockTypes.FirstOrDefault(b => b.StockTypeID == st && b.QualificationID == qu);
            if (res != null)
                return res.ColourID.ToString();
            else
                return "NoColour";
     
        }

        public ActionResult getColours()
        {
            return Json(db.Colours.Select(x => new
            {

                EstaID = x.ColourID,
                Description = x.ColourName

            }).ToList(), JsonRequestBehavior.AllowGet);


        }
        public ActionResult setPrice(string Quali,string EstablishID, string Fac, string InstituteID, string StockType, string TransactionType, string Price, string StockSize)
        {

            AddedToListItems items = new AddedToListItems();
            items.Institution = InstituteID;
            items.Faculty = Fac;

            int EstID = Convert.ToInt32(EstablishID);
            int InstituId = Convert.ToInt32(items.Institution);
            int StckType = Convert.ToInt32(StockType);
            int bb = Convert.ToInt32(StockType);
            if (Fac == "")
            {
                items.Colour = Quali;
            }
            else
            { 
                     int FacID = Convert.ToInt32(items.Faculty);
                var result = db.InstitutionFaculties.SingleOrDefault(b => b.InstitutionID == InstituId && b.FacultyID == FacID);
           

                items.Colour = getColour(StockType, Quali);

                if (items.Colour == "NoColour")
                {
                    items.Colour = result.ColourID.ToString();

                }
            
            }
               
       



                var res = db.Prices.SingleOrDefault(b => b.InstitutionID == InstituId && b.EstablishmentID == EstID && b.StockTypeID == StckType);
                string RentalPrice = "";
                string RetailPrice = "";
            string PriceiD = "";
                if (res != null)
                {

                if (res != null)
                {
                    List<Price> prices = new List<Price>();
                    prices.Add(res);
                    int count = 0;
                    DateTime dateTime = DateTime.Now;
                    foreach (var B in prices)
                    {
                        if (count == 0)
                        {
                            dateTime = B.PriceDate;
                            RentalPrice = Price = Convert.ToString(B.RentalPrice);
                            RetailPrice = Convert.ToString(res.RetailPrice);
                            PriceiD = Convert.ToString(B.PriceID);

                        }

                        count++;

                        if (DateTime.Compare(dateTime, B.PriceDate) > 0)
                        {
                           
                                RentalPrice = Price = Convert.ToString(B.RentalPrice);
                            RetailPrice = Convert.ToString(res.RetailPrice);
                            PriceiD = Convert.ToString(B.PriceID);
                        }

                    }

                }
                    
                }

                items.Price = Price;
                int SizeItem = Convert.ToInt32(StockSize);
                items.StockType = StockType;
                items.TransactionType = TransactionType;
                items.StockSize = StockSize;



                int ColourID = Convert.ToInt32(items.Colour);

                var StockDescript = db.StockDescriptions.SingleOrDefault(b => b.StockTypeID == bb && b.SizeID == SizeItem && b.ColourID == ColourID);
                ///      var HightStockDescript = db.StockDescriptions.SingleOrDefault(b => b.StockTypeID == 2 && b.SizeID == 1 && b.ColourID == 1);

                if (StockDescript == null)
                {

                    return Json("failed", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<DescriptorList> lists = new List<DescriptorList>();
                    DescriptorList descriptor = new DescriptorList();
                    var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/AddListItems.json"));
                    dynamic array = JsonConvert.DeserializeObject(jsonData);
                    lists = JsonConvert.DeserializeObject<List<DescriptorList>>(jsonData)

                            ?? new List<DescriptorList>();
                    descriptor.DescriptionID = Convert.ToString(StockDescript.StockDescriptionID);
                    descriptor.StockType = StockDescript.StockType.DESCRIPTION;
                    descriptor.StockSize = StockDescript.Size.SizeDescription;
                    descriptor.StockColour = StockDescript.Colour.ColourName;

                    if (Convert.ToInt32(TransactionType) == 2)
                    {
                        descriptor.TransactionType = "Retail";
                        descriptor.RentalPrice = RetailPrice;
                    descriptor.RetailPrice = PriceiD;

                    }
                    else
                    {
                        descriptor.TransactionType = "Rental";
                        descriptor.RentalPrice = RentalPrice;
                    descriptor.RetailPrice = PriceiD;
                }

                    Boolean flag = false;
                    if (array != null)
                    {
                        foreach (var item in array)
                        {
                            if (item.DescriptionID == descriptor.DescriptionID && item.TransactionType == descriptor.TransactionType)
                            {
                                flag = true;
                                break;
                            }

                        }

                    }

                    if (!flag)
                    {
                        lists.Add(descriptor);
                        string JsonDescription = JsonConvert.SerializeObject(lists.ToArray());
                        System.IO.File.WriteAllText(Server.MapPath("~/Scripts/AddListItems.json"), JsonDescription);
                        var JsonString = JsonConvert.SerializeObject(lists.ToArray());
                    }
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }

     
        }

        public ActionResult DeleteDescript(string DescriptID, string TransType)
        {
            var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/AddListItems.json"));
            dynamic Darray = JsonConvert.DeserializeObject(Description);
            List<DescriptorList> TempList = new List<DescriptorList>();
            Boolean flag = false;
            if (Darray != null)
            {
                foreach (var item in Darray)
                {
                    if (item.DescriptionID == DescriptID)
                    {


                        if (item.TransactionType == TransType)
                        {
                            flag = true;
                        }
                        else
                        {
                            DescriptorList descriptor = new DescriptorList();
                            descriptor.DescriptionID = item.DescriptionID;
                            descriptor.StockColour = item.StockColour;
                            descriptor.StockSize = item.StockSize;
                            descriptor.StockType = item.StockType;
                            descriptor.TransactionType = item.TransactionType;
                            descriptor.RentalPrice = item.RentalPrice;
                            descriptor.RetailPrice = item.RetailPrice;

                            TempList.Add(descriptor);
                        }
                    }
                    else
                    {
                        DescriptorList descriptor = new DescriptorList();
                        descriptor.DescriptionID = item.DescriptionID;
                        descriptor.StockColour = item.StockColour;
                        descriptor.StockSize = item.StockSize;
                        descriptor.StockType = item.StockType;
                        descriptor.TransactionType = item.TransactionType;
                        descriptor.RentalPrice = item.RentalPrice;
                        descriptor.RetailPrice = item.RetailPrice;

                        TempList.Add(descriptor);

                    }

                }

                if (flag)
                {
                    string JsonDescription = JsonConvert.SerializeObject(TempList.ToArray());
                    System.IO.File.WriteAllText(Server.MapPath("~/Scripts/AddListItems.json"), JsonDescription);
                    var JsonString = JsonConvert.SerializeObject(TempList.ToArray());
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("NoItem", JsonRequestBehavior.AllowGet);
         
            }
            else

                return Json("Item not found", JsonRequestBehavior.AllowGet);


           
        }
        public ActionResult  SaveItem(string barCode)
        {

            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barCode);
            RetrieveData d = new RetrieveData();
            Descriptions des = new Descriptions();
            string ItemPrice = "";
            if (result != null)
            {

                d.DescriptionID = Convert.ToString(result.StockDescriptionID);
                d.StockBarcode = result.StockBarcode;
                d.StockColour = result.StockDescription.Colour.ColourName;
                d.StockSize = result.StockDescription.Size.SizeDescription;
                d.Status = result.Status.StatusDescription;
                d.location = result.Location.DESCRIPTION;
                d.StockType = result.StockDescription.StockType.DESCRIPTION;
                d.Department = result.Department.DESCRIPTION;

                var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/AddListItems.json"));
                dynamic Darray = JsonConvert.DeserializeObject(Description);
                Boolean flag = false;
                Boolean flagTrans = false;
                string PriceItem = "";
                foreach (var item in Darray)
                {
              
                    if (item.DescriptionID == d.DescriptionID)
                    {
                      
                        flag = true;
                        if (item.TransactionType == d.Department)
                         {
                            ItemPrice = item.RetailPrice;
                            PriceItem = item.RentalPrice;

                            flagTrans = true;

                             break;
                         }

                    }


                }
                if (!flag)
                    return Json("Description Different", JsonRequestBehavior.AllowGet);
                else
                 if (!flagTrans)
                    return Json("Item belongs to a different transaction", JsonRequestBehavior.AllowGet);


                var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/TransactionItems.json"));
                dynamic array = JsonConvert.DeserializeObject(jsonData);

                List<RetrieveData> b = new List<RetrieveData>();

                b = JsonConvert.DeserializeObject<List<RetrieveData>>(jsonData)
                       ?? new List<RetrieveData>();
                d.Price = ItemPrice;
                b.Add(d);
                string json = JsonConvert.SerializeObject(b.ToArray());

                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/TransactionItems.json"), json);
                //write string to file

                List<RetrieveData> passData = new List<RetrieveData>();
                passData.Add(d);
                var StockItemList = from stck in passData
                                    select new
                                    {
                                        DescID = stck.DescriptionID,
                                        Bar = stck.StockBarcode,
                                        StckTy = stck.StockType,
                                        StckCOlour = stck.StockColour,
                                        StckPrice = PriceItem,
                                        TransType = stck.Department,
                                        stckSize = stck.StockSize
                                    };

            DeleteDescript(d.DescriptionID, d.Department);
                return Json(StockItemList.ToList(), JsonRequestBehavior.AllowGet);
            }

            return Json("Barcode does not exists", JsonRequestBehavior.AllowGet);

        }
        public ActionResult getStockItem(string barCode)
        {

            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barCode);
            RetrieveData d = new RetrieveData();
            Descriptions des = new Descriptions();
            string ItemPrice = "";
            if (result != null)
            {

                d.DescriptionID = Convert.ToString(result.StockDescriptionID);
                d.StockBarcode = result.StockBarcode;
                d.StockColour = result.StockDescription.Colour.ColourName;
                d.StockSize = result.StockDescription.Size.SizeDescription;
                d.Status = result.Status.StatusDescription;
                d.location = result.Location.DESCRIPTION;
                d.StockType = result.StockDescription.StockType.DESCRIPTION;
                d.Department = result.Department.DESCRIPTION;

                var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/AddListItems.json"));
                dynamic Darray = JsonConvert.DeserializeObject(Description);
                Boolean flag = false ;
                Boolean flagTrans = true;
               
                foreach (var item in Darray)
                {
                    if (item.DescriptionID == d.DescriptionID)
                    {
                        ItemPrice = item.RentalPrice;
                        flag = true;
                       /*if (item.TransactionType == d.Department)
                        {

                            flagTrans = true;
                         
                            break;
                        }*/
                     
                    }
                   

                }
                if (!flag)
                    return Json("Description Different", JsonRequestBehavior.AllowGet);
                else
                 if (!flagTrans)
                    return Json("Item belongs to a different transaction", JsonRequestBehavior.AllowGet);
                

               
                //write string to file

                List<RetrieveData> passData = new List<RetrieveData>();
                passData.Add(d);
                var StockItemList = from stck in passData
                                    select new
                                    {
                                        DescID = stck.DescriptionID,
                                        Bar = stck.StockBarcode,
                                        StckTy = stck.StockType,
                                        StckCOlour = stck.StockColour,
                                        StckPrice = ItemPrice,
                                        TransType =stck.Department,
                                        stckSize =stck.StockSize
                                    };
                return Json(StockItemList.ToList(), JsonRequestBehavior.AllowGet);
            }

            return Json("Barcode does not exists", JsonRequestBehavior.AllowGet);

        }

    
        public ActionResult ReceiveItem(string barCode)
        {

            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barCode);
            RetrieveData d = new RetrieveData();
            Descriptions des = new Descriptions();
            string ItemPrice = "";
            if (result != null)
            {

                d.DescriptionID = Convert.ToString(result.StockDescriptionID);
                d.StockBarcode = result.StockBarcode;
                d.StockColour = result.StockDescription.Colour.ColourName;
                d.StockSize = result.StockDescription.Size.SizeDescription;
                d.Status = result.Status.StatusDescription;
                d.location = result.Location.DESCRIPTION;
                d.StockType = result.StockDescription.StockType.DESCRIPTION;
                d.Department = result.Department.DESCRIPTION;

                var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/ReturnedItems.json"));
                dynamic Darray = JsonConvert.DeserializeObject(Description);

                var ScannedItems = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/ScannedItems.json"));
                dynamic Daray = JsonConvert.DeserializeObject(Description);
                var  b = JsonConvert.DeserializeObject<List<RetrieveData>>(ScannedItems)
                       ?? new List<RetrieveData>();
                Boolean flag = false;
                Boolean flagTrans = false;

                foreach (var item in Darray)
                {
                    if (item.StockBarcode == d.StockBarcode)
                    {
                        flag = true;
                            break;
                    }


                }
                if (!flag)
                    return Json("Description Different", JsonRequestBehavior.AllowGet);
                else
                 if (!flagTrans)
                    return Json("Item belongs to a different transaction", JsonRequestBehavior.AllowGet);

                List<RetrieveData> Retur = new List<RetrieveData>();
                List<RetrieveData> Scanned = new List<RetrieveData>();
                foreach (var item in Darray)
                {
                    if (item.StockBarcode != d.StockBarcode)
                    {
                        Retur.Add(item);
                    }
                }
                b.Add(d);
                var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/TransactionItems.json"));
                dynamic array = JsonConvert.DeserializeObject(jsonData);

            //    List<RetrieveData> b = new List<RetrieveData>();

                b = JsonConvert.DeserializeObject<List<RetrieveData>>(jsonData)
                       ?? new List<RetrieveData>();
                b.Add(d);
                string json = JsonConvert.SerializeObject(b.ToArray());

                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/TransactionItems.json"), json);
                //write string to file

                List<RetrieveData> passData = new List<RetrieveData>();
                passData.Add(d);
                var StockItemList = from stck in passData
                                    select new
                                    {
                                        DescID = stck.DescriptionID,
                                        Bar = stck.StockBarcode,
                                        StckTy = stck.StockType,
                                        StckCOlour = stck.StockColour,
                                        StckPrice = ItemPrice,
                                        TransType = stck.Department,
                                        stckSize = stck.StockSize
                                    };
                return Json(StockItemList.ToList(), JsonRequestBehavior.AllowGet);
            }

            return Json("Barcode does not exists", JsonRequestBehavior.AllowGet);

        }
        public ActionResult addToList(FormCollection form)
        {

            AddedToListItems items = new AddedToListItems();
            items.Institution = form["Institutes"].ToString();
            items.Faculty = form["Faculty"].ToString();
            int InstituId = Convert.ToInt32(items.Institution);
            int FacID = Convert.ToInt32(items.Faculty);
            var result = db.InstitutionFaculties.SingleOrDefault(b => b.InstitutionID == InstituId && b.FacultyID == FacID);
            items.Colour = result.ColourID.ToString();
            items.Price = PriceItem;
            items.StockSize = form["SizeCont"].ToString(); 
       
            items.StockType = form["StockType"].ToString(); 
            items.TransactionType = form["TransactionType"].ToString();

            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/AddListItems.json"));
            dynamic array = JsonConvert.DeserializeObject(jsonData);
            var employeeList = JsonConvert.DeserializeObject<List<AddedToListItems>>(jsonData)
                    ?? new List<AddedToListItems>();
            employeeList.Add(items);
            string JsonDescription = JsonConvert.SerializeObject(employeeList.ToArray());
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/AddListItems.json"), JsonDescription);
            return Json(PriceItem, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getDepartment()
        { 
            return Json(db.Departments.Select(x=> 
            new { 
                DepID = x.DepartmentID,
                description = x.DESCRIPTION
            }).ToList(),JsonRequestBehavior.AllowGet);
        
        }

        public ActionResult getTotalPrice(string Price,string CurrentPrice)
        {
            decimal TotalPrice = Convert.ToDecimal(Price);
            decimal Curr = Convert.ToDecimal(CurrentPrice);
            TotalPrice += Curr;
            return Json(TotalPrice, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getFaculties()
        {
            var Sizes = from Institut in db.Institutions
                        from Fac in Institut.Faculties
                        select new
                        {
                            InstID = Institut.InstitutionID,
                            description =Fac.Description,
                           FacID = Fac.FacultyID
                        };

            if (Sizes == null)
                return Json("NoItem", JsonRequestBehavior.AllowGet);
            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult getQualification()
        {
            var Sizes = from Institute in db.Institutions
                        from Fac in Institute.Faculties
                        from b in Fac.FacultyQualifications
                        select new 
                        {
                            InstID = Institute.InstitutionID,
                            FacID = Fac.FacultyID,
                            description = b.Qualification.QualificationName,
                            QualID = b.QualificationID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult getSize(string StypeID)
        {
            var Sizes = from StockType in db.StockTypes
                        from Size in StockType.Sizes
                        
                        select new
                        {
                            SizeDesc = Size.SizeDescription,
                            ID = Size.SizeID,
                            stockTypeID = StockType.StockTypeID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);

        }
        public ActionResult getHeadCircum()
        {
            var Sizes = from StockType in db.StockTypes
                        from Size in StockType.Sizes

                        select new
                        {
                            SizeDesc = Size.SizeDescription,
                            ID = Size.SizeID,
                            stockTypeID = StockType.StockTypeID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult getHeight()
        {
            var Sizes = from Institut in db.Institutions
                        from Fac in Institut.Faculties
                        select new
                        {
                            InstID = Institut.InstitutionID,
                            description = Fac.Description,
                            FacID = Fac.FacultyID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult getStockDescription(int StckType)
        {
        
            var HeadStockDescript = db.StockDescriptions.SingleOrDefault(b => b.StockTypeID == 1 && b.SizeID == 1 && b.ColourID == 1);
            var HightStockDescript = db.StockDescriptions.SingleOrDefault(b => b.StockTypeID == 2 && b.SizeID == 1 && b.ColourID == 1);

            if (StckType == 1)
                return Json(HeadStockDescript,JsonRequestBehavior.AllowGet);
            else
                return Json(HightStockDescript, JsonRequestBehavior.AllowGet);
        }

        public string GenerateCustomerRentalID()
        {
          
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < 6; i++)
                s = String.Concat(s, random.Next(1, 9).ToString());
            
            return s;


        }


        public void UpdateItemStatus(string barcode, int Statu)
        {
            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barcode);
            result.StockStatusID = Statu;
            db.SaveChanges();
        }
        public ActionResult PlaceOrder(string CustID, string Name,string RentalAmount, string RetailAmount)
        {
            List<CustomerDetails> customerDetails = new List<CustomerDetails>();
            CustomerDetails details = new CustomerDetails();
            int customeID = Convert.ToInt32(CustID);
            Decimal TotalCost = Convert.ToDecimal(RentalAmount);
            var AddList = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/AddListItems.json"));
            dynamic Darray = JsonConvert.DeserializeObject(AddList);
            string CustRent;
            string CustPurch;
            Boolean flag = false;
            Boolean rentalFlag = false;
            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/TransactionItems.json"));
            dynamic array = JsonConvert.DeserializeObject(jsonData);
            decimal RentalTotal = 0;
            decimal RetailTotal = 0;
            if (array != null)
            {
                foreach (var item in array)
                {
                    if (item.Department == "Rental")
                    {
                        int Coun = Convert.ToInt32(item.Price);
                        var r = db.Prices.Find(Coun);
                        if (r != null)
                            RentalTotal += r.RentalPrice;
                        rentalFlag = true;
                    }
                    if (item.Department == "Retail")
                    {
                        int Coun = Convert.ToInt32(item.Price);
                        var r = db.Prices.Find(Coun);
                        if (r != null)
                            RetailTotal += r.RetailPrice;

                        flag = true;
                    }
                }
            }
            else
            {
                return Json("failed", JsonRequestBehavior.AllowGet);
            }

            if (rentalFlag)
            {

                string customerRentalID = GenerateCustomerRentalID();
                details.RentalID = customerRentalID;
                CustRent = customerRentalID;
                CustomerRental customer = new CustomerRental();
                customer.CustomerRentalID = customerRentalID;
                customer.CustomerID = customeID;
                customer.OrderStatusID = 1;
                customer.PaymentMethodID = 1;
                customer.OrderDateTime = DateTime.Now;
                customer.AmountPaid = RentalTotal;
             
                db.CustomerRentals.Add(customer);
                db.SaveChanges();

                foreach (var item in array)
                {
                    if (item.Department == "Rental")
                    {
                        CustomerRentalLine customerRental = new CustomerRentalLine();
                        customerRental.CustomerRentalID = customer.CustomerRentalID;
                        string barcode = item.StockBarcode;
                        var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barcode);
                        customerRental.StockID = result.StockID;
                        int StID = result.StockDescription.StockTypeID;
                      int PricID = Convert.ToInt32(item.Price);
                        customerRental.PriceID = PricID;
                        db.CustomerRentalLines.Add(customerRental);
                        db.SaveChanges();
                        UpdateItemStatus(barcode,2);
                        UpdateRentalDescription(result.StockDescriptionID, 1);
                    }
              

                  }
            }


            if (flag)
            {
                string PurchID = GenerateCustomerRentalID();
                CustPurch = PurchID;
                details.PurchaseID = PurchID;
                PurchaseOrder purchase = new PurchaseOrder();
                purchase.PurchaseOrderID = PurchID;
                purchase.CustomerID = customeID;
                purchase.OrderStatusID = 1;
                purchase.PaymentMethodID =1;

                purchase.OrderDateTime = DateTime.Now;
                purchase.AmountPaid = RetailTotal;
                db.PurchaseOrders.Add(purchase);
                db.SaveChanges();

                foreach (var item in array)
                {
                    if (item.Department == "Retail")
                    {
                        PurchaseOrderLine customerRental = new PurchaseOrderLine();
                        customerRental.PurchaseOrderID = purchase.PurchaseOrderID;
                        string barcode = item.StockBarcode;
                        var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barcode);
                        customerRental.StockID = result.StockID;
                        int StID = result.StockDescription.StockTypeID;
                        int PricID = Convert.ToInt32(item.Price);
                        customerRental.PriceID = PricID;
                        db.PurchaseOrderLines.Add(customerRental);
                        db.SaveChanges();
                        UpdateItemStatus(barcode, 3);
                        UpdateRentalDescription(result.StockDescriptionID, 2);
                    }


                }


            }

            if (flag || rentalFlag)
            {
                details.CustomerName = Name;
                details.EmployeeName = "Mr Mahlangu ";
                customerDetails.Add(details);
                return Json(PrepareReport(customerDetails), JsonRequestBehavior.AllowGet);
            }
         
           
            return null; 
        }



        public void UpdateRentalDescription(int id, int dep)
        {
            var result = db.StockDescriptions.FirstOrDefault(b => b.StockDescriptionID == id);
            if (result != null)
            {
                if (dep == 1)
                    result.RENTALSTOCKLEVEL -= 1;
                else
                    result.RETAILSTOCKLEVEL -= 1;

                db.SaveChanges();
            }

        }











        // GET: CustomerRental/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerRental customerRental = db.CustomerRentals.Find(id);
            if (customerRental == null)
            {
                return HttpNotFound();
            }
            return View(customerRental);
        }

        // GET: CustomerRental/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            ViewBag.DeliveryID = new SelectList(db.Deliveries, "DeliveryID", "DeliveryAddress");
            ViewBag.OrderStatusID = new SelectList(db.OrderStatus, "OrderStatusID", "DESCRIPTION");
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "Description");
            return View();
        }

        // POST: CustomerRental/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerRentalID,CustomerID,OrderStatusID,DeliveryID,PaymentMethodID,OrderDateTime,AmountPaid")] CustomerRental customerRental)
        {
            if (ModelState.IsValid)
            {
                db.CustomerRentals.Add(customerRental);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", customerRental.CustomerID);
            ViewBag.DeliveryID = new SelectList(db.Deliveries, "DeliveryID", "DeliveryAddress", customerRental.DeliveryID);
            ViewBag.OrderStatusID = new SelectList(db.OrderStatus, "OrderStatusID", "DESCRIPTION", customerRental.OrderStatusID);
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "Description", customerRental.PaymentMethodID);
            return View(customerRental);
        }

        // GET: CustomerRental/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerRental customerRental = db.CustomerRentals.Find(id);
            if (customerRental == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", customerRental.CustomerID);
            ViewBag.DeliveryID = new SelectList(db.Deliveries, "DeliveryID", "DeliveryAddress", customerRental.DeliveryID);
            ViewBag.OrderStatusID = new SelectList(db.OrderStatus, "OrderStatusID", "DESCRIPTION", customerRental.OrderStatusID);
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "Description", customerRental.PaymentMethodID);
            return View(customerRental);
        }

        // POST: CustomerRental/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerRentalID,CustomerID,OrderStatusID,DeliveryID,PaymentMethodID,OrderDateTime,AmountPaid")] CustomerRental customerRental)
        {
            if (ModelState.IsValid)
            {
             
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", customerRental.CustomerID);
            ViewBag.DeliveryID = new SelectList(db.Deliveries, "DeliveryID", "DeliveryAddress", customerRental.DeliveryID);
            ViewBag.OrderStatusID = new SelectList(db.OrderStatus, "OrderStatusID", "DESCRIPTION", customerRental.OrderStatusID);
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "Description", customerRental.PaymentMethodID);
            return View(customerRental);
        }

        // GET: CustomerRental/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerRental customerRental = db.CustomerRentals.Find(id);
            if (customerRental == null)
            {
                return HttpNotFound();
            }
            return View(customerRental);
        }

        // POST: CustomerRental/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomerRental customerRental = db.CustomerRentals.Find(id);
            db.CustomerRentals.Remove(customerRental);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
