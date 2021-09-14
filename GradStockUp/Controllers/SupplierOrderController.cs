using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
namespace GradStockUp.Controllers
{
    
    public class SupplierOrderController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();
        // GET: SupplierOrder
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult PlaceSupplierOrder()
        {
            //1. Supplier DropdownList
            List<Supplier> suppliersList = db.Suppliers.ToList();
            ViewData["Types"] = suppliersList;

            //2. StockType DropdownList
            List<StockType> stkTypeList = db.StockTypes.ToList();
            ViewData["StkTypes"] = stkTypeList;
            //3. Colour DropdownList
            List<Colour> colorList = db.Colours.ToList();
            ViewData["Colours"] = colorList;
            return View();
        }

        [HttpPost]
        public ActionResult PlaceSupplierOrder(IEnumerable<SupplierOrderDetailsModel> listofSupplierOrderDetailModels)
        {
            if (listofSupplierOrderDetailModels!=null)
            {
                var supplierName = listofSupplierOrderDetailModels.First().SupplierName;
                var supplier = db.Suppliers.First(item => string.Equals(item.SupplierName.ToLower(), supplierName.ToLower()));

                //        //1. Create New Order inside db
                var newOrder = new SupplierOrder
                {
                    OrderStatusID = 1,
                    DateCreated = DateTime.Now,
                    TotalCost = Convert.ToDecimal(0),
                    SupplierID = supplier.SupplierID
                };
                db.SupplierOrders.Add(newOrder);
                db.SaveChanges();

                //Recent OrderID
                int SupOID = newOrder.SupplierOrderID;


                foreach (var listItem in listofSupplierOrderDetailModels)
                {
                    var stockType = db.StockTypes.First(item => string.Equals(item.DESCRIPTION.ToLower(), listItem.StockType));
                    var colur = db.Colours.First(item => string.Equals(item.ColourName.ToLower(), listItem.Colour));
                    var size = db.Sizes.First(item => string.Equals(item.SizeDescription.ToLower(), listItem.Size));


                    //2. Stockdescriptions added
                    var result = db.StockDescriptions
                         .Where(s => s.StockTypeID == stockType.StockTypeID && s.ColourID == colur.ColourID && s.SizeID == size.SizeID).FirstOrDefault();
                    //var newResult = from m in db.StockDescriptions
                    //                select m

                    if (result == null)
                    {

                        var newDescription = new StockDescription
                        {
                            ColourID = colur.ColourID,
                            SizeID = size.SizeID,
                            StockTypeID = stockType.StockTypeID,
                            FLAG = true,
                            RENTALSTOCKLEVEL = 0,
                            RETAILSTOCKLEVEL = 0,
                            RENTALTHRESHOLD = 15,
                            RETAILTHRESHOLD = 15
                        };
                        db.StockDescriptions.Add(newDescription);
                        db.SaveChanges();
                        var NewList = newDescription.ToString();
                        var orderLine = new SupplierOrderLine
                        {
                            Quantity = Convert.ToInt32(listItem.Quantity),
                            StockDescriptionID = newDescription.StockDescriptionID,
                            SupplierOrderID = newOrder.SupplierOrderID
                        };

                        db.SupplierOrderLines.Add(orderLine);
                        db.SaveChanges();
                    }
                    else
                    {
                        int StockDescrptiID = result.StockDescriptionID;
                        var orderLine = new SupplierOrderLine
                        {
                            Quantity = Convert.ToInt32(listItem.Quantity),
                            StockDescriptionID = StockDescrptiID,
                            SupplierOrderID = newOrder.SupplierOrderID
                        };
                        db.SupplierOrderLines.Add(orderLine);
                        db.SaveChanges();
                    }
                }


                string json = JsonConvert.SerializeObject(listofSupplierOrderDetailModels);
                //Remove Json Quotes, Commas and Other Brackets
                var EditedList = json.Replace("[", "").Replace("]", "\n").Replace("}", "\n").Replace("{", "\n").Replace("\"", "").
                    Replace(",", ", ").Replace("SupplierName", "").Replace(":", ": ").Replace(": JQAM", "").Replace(": MacD", "").
                    Replace(": DexerGrad Supplies", "").Replace(": RapRav Make", "");
                // var details = JsonConvert.DeserializeObject(json);
                var NewNewBody = "Good Day" + "\n" + "Please Find our Order Details Below" + "\n"
                                + EditedList.ToString();
                try
                {
                    if (ModelState.IsValid)
                    {


                        var senderEmail = new MailAddress("Raphaeles2016@gmail.com", "Dippenaar & Reinecke");//Insert Email Address
                        var receiverEmail = new MailAddress(supplier.EMAIL, "Recipient");
                        var password = "0748743346"; //Insert EmailAccount Password
                        var sub = "New Order";
                        var body =
                              NewNewBody.ToString();
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = sub,
                            Body = body
                        })
                        {
                            smtp.Send(mess);
                        }


                        return View("SuccessSupplierOrder");
                    }
                }
                catch (Exception)
                {

                    ViewBag.Error = "Some Error";
                 
                }
                
            }
            else
            {
                ViewBag.ErrorMessage = "Error: cannot place order without items in the cart, please Add items to Cart";

            }
            return View();
        }
        public ActionResult GetSize()
        {
            var Sizes3 = from StockType in db.StockTypes
                         from Size in StockType.Sizes
                         select new
                         {
                             SizeDesc = Size.SizeDescription,
                             ID = Size.SizeID,
                             StockTypeID2 = StockType.StockTypeID

                         };
            ViewBag.SizeOptions = new SelectList(Sizes3, "SizeID", "SizeDescription");

            return Json(Sizes3.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SuccessSupplierOrder()
        {
            return View();
        }
    }
}