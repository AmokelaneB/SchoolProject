using GradStockUp.Models;
using GradStockUp.View_Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Deployment.Internal;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace GradStockUp.Controllers
{
    public class OnlineOrderController : Controller
    {

        GradStockUpEntities db = new GradStockUpEntities();
        // GET: OnlineOrder
        public ActionResult Index()
        {

            var OrderAndCustomerList = db.Customers.Where(x => x.OnlineOrders.Count >= 1);
            return View(OrderAndCustomerList.ToList());
        }
        //GET: OnlineOrder/SendforDel/5


        public ActionResult SendforDel(int? id)
        {
            DateTime date = DateTime.Now;
            DayOfWeek day = DateTime.Now.DayOfWeek;
            string dayToday = day.ToString();

            if (ModelState.IsValid)
            {
                if (id != null)
                {

                    Customer customer = db.Customers.Find(id);
                    var onlinorder = db.OnlineOrders.Where(x => x.CustomerID == customer.CustomerID).ToList();
                    var _pd = db.PurchaseOrders.Where(x => x.CustomerID == customer.CustomerID).ToList();
                    var crd = db.CustomerRentals.Where(x => x.CustomerID == customer.CustomerID).ToList();


                    Delivery del = new Delivery();
                    del.DeliveryStatusID = 1;
                    del.DeliveryAddress = customer.CustAddress;
                    del.DeliveryDateTime = date.AddDays(5);
                    db.Deliveries.Add(del);
                    db.SaveChanges();

                    foreach (var item in onlinorder)
                    {
                        item.OrderStatusID = 2;

                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();

                        if (item.OrderType == "Retail")
                        {
                            foreach (var pdo in _pd)
                            {
                                pdo.DeliveryID = del.DeliveryID;
                                db.Entry(pdo).State = EntityState.Modified;

                                db.SaveChanges();
                            }
                        }
                        else if (item.OrderType == "Rental")
                        {
                            foreach (var cdo in crd)
                            {
                                cdo.DeliveryID = del.DeliveryID;

                                db.Entry(cdo).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }

                    foreach (var item in _pd)
                    {
                        item.DeliveryID = del.DeliveryID;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    Event _e = new Event();
                    _e.start_date = date;
                    _e.end_date = date.AddDays(5);
                    _e.text = $"Delivery: Deliver Ordered Items to {customer.CustomerSurname} {customer.CustomerName} at {del.DeliveryAddress} on {del.DeliveryDateTime}";
                    db.Events.Add(_e);
                    db.SaveChanges();

                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("amokelanebaloh10@gmail.com", "holynation"),
                        EnableSsl = true,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("amokelanebaloh10@gmail.com"),
                        Subject = "Order Sent For Delivery",
                        Body = "<h1>Hello " + customer.CustomerName + "</h1><br/><p>Expect your order to be delivered in 5 days </p> <p>Regards,<br/>Dipenaar and Reneicke</p><hr/> <br/><footer><p> &copy GradStockUp </p > </ footer >",
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(customer.Email);

                    smtpClient.Send(mailMessage);

                    TempData["SuccessMessage"] = "Order Sent For Delivery";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Customer Does Not Exit";
                    return View("Index");
                }
            }
            TempData["SuccessMessage"] = "Order Sent For Delivery";
            return View("Index");
        }

        public ActionResult getStockItem(string barCode)
        {


            RetreveItemData d = new RetreveItemData();

            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barCode);
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

                var jsonToWrite = JsonConvert.SerializeObject(d, Formatting.Indented);

                using (var writer = new StreamWriter(Server.MapPath("~/Scripts/AddListItems2.json")))
                {
                    writer.Write(jsonToWrite);
                }

                string jsonFromFile;

                using (var reader = new StreamReader(Server.MapPath("~/Scripts/AddListItems2.json")))
                {
                    jsonFromFile = reader.ReadToEnd();
                }

                var _s = JsonConvert.DeserializeObject<RetreveItemData>(jsonFromFile);

                List<RetreveItemData> orderItem = new List<RetreveItemData>();
                orderItem.Add(d);

                var StockItemList = from stck in orderItem
                                    select new
                                    {
                                        _desID = stck.DescriptionID,
                                        Barcode = stck.StockBarcode,
                                        StckTy = stck.StockType,
                                        StckCOlour = stck.StockColour,
                                        TransType = stck.Department,
                                        stckSize = stck.StockSize
                                    };
                return Json(StockItemList.ToList(), JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json("Barcode does not exist", JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult PostScannedItems(IEnumerable<Stock> retreveItemDatas)
        {

            if (retreveItemDatas != null)
            {
                foreach (var item in retreveItemDatas)
                {
                    var _si = db.Stocks.Where(x => x.StockBarcode == item.StockBarcode).FirstOrDefault();
                    if (_si.DepartmentID == 1)
                    {
                        _si.StockStatusID = 2;
                        db.Entry(_si).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (_si.DepartmentID == 2)
                    {
                        _si.StockStatusID = 3;
                        db.Entry(_si).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return Json("Scan Complete", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("No Items scanned", JsonRequestBehavior.AllowGet);
            }

        }
    }
}