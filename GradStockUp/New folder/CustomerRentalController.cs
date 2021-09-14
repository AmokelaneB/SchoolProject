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

            return View();

        }


        public ActionResult getCustomerOrder(string customerRentalID)
        {
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/ReturnedItems.json"), string.Empty);
            string RentalID = customerRentalID;
            var result = db.CustomerRentals.SingleOrDefault(b => b.CustomerRentalID == RentalID);

            if (result != null)
            {
                List<Customer> customers = new List<Customer>();
                var CustomerResult = db.Customers.SingleOrDefault(b => b.CustomerID == result.CustomerID);
                customers.Add(CustomerResult);

                List<int> StockIDs = new List<int>();


                var customerRentals = db.CustomerRentalLines.Select(x => new {
                    CustRentID = x.CustomerRentalID,
                    StckID = x.StockID

                }).ToList();
                int count = 0;
                foreach (var item in customerRentals)
                {
                    if (item.CustRentID == RentalID)
                    {
                        StockIDs.Add(item.StckID);
                        count++;
                    }
                }

                List<RetrieveData> rd = new List<RetrieveData>();

                foreach (int IDs in StockIDs)
                {
                    RetrieveData data = new RetrieveData();
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
                        rd.Add(data);
                    }


                }
                string json = JsonConvert.SerializeObject(rd.ToArray());

                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/ReturnedItems.json"), json);

                var cb = from b in customers
                         select new
                         {
                             CustName = b.CustomerName + b.CustomerSurname,
                             OrderDate = result.OrderDateTime,
                             Items = count,
                             RentId = RentalID
                         };
                return Json(cb.ToList(), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public ActionResult SaveCustomer(FormCollection form)
        {
            string CustomerID = form["CustomerID"].ToString();
            Customer customer = new Customer();
            customer.OrganizationID = null;
            customer.IDnumber = CustomerID;
            customer.CustomerName = form["CustName"].ToString();
            customer.CustomerSurname = form["CustSurn"].ToString();
            customer.Email = form["CustEmail"].ToString();
            customer.CustAddress = form["CustAddAddres"].ToString();
            customer.PhoneNumber = form["PhoneNum"].ToString();
            customer.NextoFKin = form["NextName"].ToString();
            customer.kinPhone = form["NextPhoneNum"].ToString();
            db.Customers.Add(customer);
            db.SaveChanges();

            return Json("success");
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
                    ID = x.CustomerID,
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
                    Price = Convert.ToString(result.RentalPrice);
                else
                    Price = Convert.ToString(result.RetailPrice);
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



        public ActionResult setPrice(string EstablshID, string hood, string Fac, string InstituteID, string StockType, string TransactionType, string Price, string HeadSize, string HeightSize)
        {
            List<DescriptorList> lists = new List<DescriptorList>();


            /* For a Cap*/

            /* Find Prices*/

            int InstitID = Convert.ToInt32(InstituteID);
            int EstID = Convert.ToInt32(EstablshID);
            int StckID = 1;
            var res = db.Prices.SingleOrDefault(b => b.InstitutionID == InstitID && b.EstablishmentID == EstID && b.StockTypeID == StckID);
            string RentalPrice = "";
            string RetailPrice = "";
            if (res != null)
            {
                RentalPrice = Price = Convert.ToString(res.RentalPrice); ;
                RetailPrice = Convert.ToString(res.RetailPrice);
            }
            PriceItem = Price;


            /*===============================*/
            /* Find the Stock Details*/
            AddedToListItems items = new AddedToListItems();

            items.Institution = InstituteID;
            items.Faculty = Fac;
            int InstituId = Convert.ToInt32(items.Institution);
            int FacID = Convert.ToInt32(items.Faculty);
            var result = db.InstitutionFaculties.SingleOrDefault(b => b.InstitutionID == InstituId && b.FacultyID == FacID);

            int SizeItem = Convert.ToInt32(HeadSize);
            int ColourID = 5;
            var StockDescript2 = db.StockDescriptions.SingleOrDefault(b => b.StockTypeID == 1 && b.SizeID == SizeItem && b.ColourID == ColourID);
            if (StockDescript2 != null)
            {
                DescriptorList descriptorList2 = new DescriptorList();
                descriptorList2.DescriptionID = Convert.ToString(StockDescript2.StockDescriptionID);
                descriptorList2.StockType = StockDescript2.StockType.DESCRIPTION;
                descriptorList2.StockColour = StockDescript2.Colour.ColourName;
                descriptorList2.StockSize = StockDescript2.Size.SizeDescription;
                descriptorList2.RentalPrice = RentalPrice;
                descriptorList2.RetailPrice = RetailPrice;
                descriptorList2.TransactionType = "Rental";
                lists.Add(descriptorList2);
            }

            /* ========================================*/


            /* For a Gown*/

            /* Find Prices*/

            InstitID = Convert.ToInt32(InstituteID);
            EstID = Convert.ToInt32(EstablshID);
            StckID = 2;
            res = db.Prices.SingleOrDefault(b => b.InstitutionID == InstitID && b.EstablishmentID == EstID && b.StockTypeID == StckID);
            RentalPrice = "";
            RetailPrice = "";
            if (res != null)
            {
                RentalPrice = Price = Convert.ToString(res.RentalPrice); ;
                RetailPrice = Convert.ToString(res.RetailPrice);
            }
            PriceItem = Price;


            /*===============================*/
            /* Find the Stock Details*/
            items = new AddedToListItems();

            items.Institution = InstituteID;
            items.Faculty = Fac;
            InstituId = Convert.ToInt32(items.Institution);
            FacID = Convert.ToInt32(items.Faculty);
            result = db.InstitutionFaculties.SingleOrDefault(b => b.InstitutionID == InstituId && b.FacultyID == FacID);

            SizeItem = Convert.ToInt32(HeightSize);
            ColourID = 5;
           var StockDescript1 = db.StockDescriptions.SingleOrDefault(b => b.StockTypeID == 2 && b.SizeID == SizeItem && b.ColourID == ColourID);
            if (StockDescript1 != null)
            {
                DescriptorList descriptorList1 = new DescriptorList();
                descriptorList1.DescriptionID = Convert.ToString(StockDescript1.StockDescriptionID);
                descriptorList1.StockType = StockDescript1.StockType.DESCRIPTION;
                descriptorList1.StockColour = StockDescript1.Colour.ColourName;
                descriptorList1.StockSize = StockDescript1.Size.SizeDescription;
                descriptorList1.RentalPrice = RentalPrice;
                descriptorList1.RetailPrice = RetailPrice;
                descriptorList1.TransactionType = "Rental";
                lists.Add(descriptorList1);
            }

            /* ========================================*/

            /* For a Hood*/

            /* Find Prices*/

            InstitID = Convert.ToInt32(InstituteID);
            EstID = Convert.ToInt32(EstablshID);
            StckID = 2;
            res = db.Prices.SingleOrDefault(b => b.InstitutionID == InstitID && b.EstablishmentID == EstID && b.StockTypeID == StckID);
            RentalPrice = "";
            RetailPrice = "";
            if (res != null)
            {
                RentalPrice = Price = Convert.ToString(res.RentalPrice); ;
                RetailPrice = Convert.ToString(res.RetailPrice);
            }
            PriceItem = Price;


            /*===============================*/
            /* Find the Stock Details*/
            items = new AddedToListItems();

            items.Institution = InstituteID;
            items.Faculty = Fac;
            InstituId = Convert.ToInt32(items.Institution);
            FacID = Convert.ToInt32(items.Faculty);
            result = db.InstitutionFaculties.SingleOrDefault(b => b.InstitutionID == InstituId && b.FacultyID == FacID);

            SizeItem = Convert.ToInt32(hood);
            ColourID = result.ColourID;
           var StockDescript = db.StockDescriptions.SingleOrDefault(b => b.StockTypeID == 3 && b.SizeID == SizeItem && b.ColourID == ColourID);
            if (StockDescript != null)
            {
                DescriptorList descriptorList = new DescriptorList();
                descriptorList.DescriptionID = Convert.ToString(StockDescript.StockDescriptionID);
                descriptorList.StockType = StockDescript.StockType.DESCRIPTION;
                descriptorList.StockColour = StockDescript.Colour.ColourName;
                descriptorList.StockSize = StockDescript.Size.SizeDescription;
                descriptorList.RentalPrice = RentalPrice;
                descriptorList.RetailPrice = RetailPrice;
                descriptorList.TransactionType = "Rental";
                lists.Add(descriptorList);
            }

            /* ========================================*/
            string JsonDescription = JsonConvert.SerializeObject(lists.ToArray());
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/AddListItems.json"), JsonDescription);
            var JsonString = JsonConvert.SerializeObject(lists.ToArray());
            return Json("Success", JsonRequestBehavior.AllowGet);
     
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
            items.HeadSize = form["HeadCircum"].ToString(); 
            items.HeightSize = form["Height"].ToString();
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
        public ActionResult PlaceOrder(string CustID, string Name,string Amount)
        {
            List<CustomerDetails> customerDetails = new List<CustomerDetails>();
            CustomerDetails details = new CustomerDetails();
            int customeID = Convert.ToInt32(CustID);
            Decimal TotalCost = Convert.ToDecimal(Amount);
            var AddList = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/AddListItems.json"));
            dynamic Darray = JsonConvert.DeserializeObject(AddList);
            string CustRent;
            string CustPurch;
            Boolean flag = false;
            Boolean rentalFlag = false;
            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/TransactionItems.json"));
            dynamic array = JsonConvert.DeserializeObject(jsonData);
             if (array != null)
            { 
                foreach (var item in array)
                {
                    if (item.Department =="Rental")
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
                customer.OrderStatusID = 3;
                customer.PaymentMethodID = 2;
                customer.OrderDateTime = DateTime.Now;
                customer.AmountPaid = TotalCost;
                customer.DeliveryID = 1;
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
                          
                        customerRental.PriceID = 2;
                        db.CustomerRentalLines.Add(customerRental);
                        db.SaveChanges();
                        UpdateItemStatus(barcode,3);
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
                purchase.OrderStatusID = 3;
                purchase.PaymentMethodID = 2;
                purchase.DeliveryID = 1;
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
                        customerRental.PriceID = 2;
                        db.PurchaseOrderLines.Add(customerRental);
                        db.SaveChanges();
                        UpdateItemStatus(barcode, 2);
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
