using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using GradStockUp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using GradStockUp.Reporting;

namespace GradStockUp.Controllers
{
    public class StockController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Stock
        public ActionResult Index()
        {
            var stocks = db.Stocks.Include(s => s.Department).Include(s => s.Location).Include(s => s.Status).Include(s => s.StockDescription).Include(s => s.StockState);
            return View(stocks.ToList());
        }


        public ActionResult getStockType()
        {
            return Json(db.StockTypes.Select(x => new
            {

                StockTypeID = x.StockTypeID,
                Description = x.DESCRIPTION
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult getSize(string StypeID)
        {
            var Sizes = from StockType in db.StockTypes
                        from Size in StockType.Sizes
                        select new
                        {
                            SizeDesc = Size.SizeDescription,
                            ID =Size.SizeID,
                            stockTypeID = StockType.StockTypeID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult getColour()
        {
            return Json(db.Colours.Select(x => new
            {

                ID = x.ColourID,
                Description = x.ColourName
            }).ToList(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult SaveDescription(string StockTypeID, string SizeID, string ColourID, string Rthresh, string Rethresh)
        {

            StockDescription stockDescription = new StockDescription();
            stockDescription.StockTypeID = Convert.ToInt32(StockTypeID);
            stockDescription.SizeID = Convert.ToInt32(SizeID);
            stockDescription.ColourID = Convert.ToInt32(ColourID);
            stockDescription.RENTALSTOCKLEVEL = 0;
            stockDescription.RENTALTHRESHOLD = Convert.ToInt32(Rthresh);
            stockDescription.RETAILSTOCKLEVEL = 0;
            stockDescription.RETAILTHRESHOLD = Convert.ToInt32(Rethresh);
            stockDescription.FLAG = false;
            db.StockDescriptions.Add(stockDescription);
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Transfer()
        {
      
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/StockDescriptions.json"), string.Empty);
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/Description.json"), string.Empty);
            return View();
        }

        public ActionResult getStockItem(string barCode)
        {
          
            var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == barCode);
            RetrieveData d = new RetrieveData();
            Descriptions des = new Descriptions();
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
                List<RetrieveData> b = new List<RetrieveData>();
                b.Add(d);

                des.DescriptionID = d.DescriptionID;
                des.StockColour = d.StockColour;
                des.StockSize = d.StockSize;
                des.StockType = d.StockType;
                des.Quantity = 1;
                var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/StockDescriptions.json"));
                dynamic array = JsonConvert.DeserializeObject(jsonData);


                var Description = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/Description.json"));
                var employeeList = JsonConvert.DeserializeObject<List<RetrieveData>>(jsonData)
                      ?? new List<RetrieveData>();
                var description = JsonConvert.DeserializeObject<List<Descriptions>>(Description)
                      ?? new List<Descriptions>();
                dynamic desArray = JsonConvert.DeserializeObject(Description);
                Boolean flag = false;
                Boolean bflag = false;
                if (array != null)
                {

                    foreach (var item in array)
                    {
                        if (item.StockBarcode == d.StockBarcode)
                        {
                            flag = true;
                            break;
                        }
                     
                    }
                }
  

                if (desArray != null)
                {
                    foreach (var Obj in description)
                    {
                        if (flag != true)
                        {
                            if (Obj.DescriptionID == d.DescriptionID)
                            {
                                Obj.Quantity += 1;
                                bflag = true;
                            }

                        }

                    }
                }

                if (!bflag)
                    description.Add(des);
                if (!flag)
                    employeeList.Add(d);

                string json = JsonConvert.SerializeObject(employeeList.ToArray());
                string JsonDescription = JsonConvert.SerializeObject(description.ToArray());
                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/Description.json"), JsonDescription);
                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/StockDescriptions.json"), json);
                //write string to file
               

            }
            
            return Json(d, JsonRequestBehavior.AllowGet);

        }


        public ActionResult TransferItems(string transferType)
        {
            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/StockDescriptions.json"));
            var employeeList = JsonConvert.DeserializeObject<List<RetrieveData>>(jsonData)
                     ?? new List<RetrieveData>();
            if (transferType == "FrontStore")
            {
                foreach (var Obj in employeeList)
                {
                    var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == Obj.StockBarcode);
                    if (result != null)
                    {
                        result.LocationID = 1;
                        db.SaveChanges();
                    }
                }
                return Json("Successfully transfered to Front store", JsonRequestBehavior.AllowGet);
            }
            else
            if (transferType == "BackStore")
            {
                foreach (var Obj in employeeList)
                {
                    var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == Obj.StockBarcode);
                    if (result != null)
                    {
                        result.LocationID = 2;
                        db.SaveChanges();
                    }
                }
                return Json( "Successfully transfered to back store", JsonRequestBehavior.AllowGet);

            }
            else
            if (transferType == "InLaundry")
            {
                int count = 0;
                foreach (var Obj in employeeList)
                {
                    count++;
                }

               CheckInLaundry checkIn = new CheckInLaundry();
                checkIn.Quantity = count;
                checkIn.CheckInDate = DateTime.Today;
                db.CheckInLaundries.Add(checkIn);
                db.SaveChanges();
                foreach (var Obj in employeeList)
                {
                    var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == Obj.StockBarcode);
                    if (result != null)
                    {
                        result.StockStatusID = 2;
                        db.SaveChanges();
                        LaundryLine laundry = new LaundryLine();
                        laundry.CheckInLaundryID = checkIn.CheckInLaundryID;
                        laundry.StockID = result.StockID;

                        laundry.StockStatusID = result.StockStatusID;
                        db.LaundryLines.Add(laundry);
                        db.SaveChanges();
                       
                    }
                }
                return Json("Successfully transfered to Laundry", JsonRequestBehavior.AllowGet);
            }
            else
            if (transferType == "OutLaundry")
            {
                int count = 0;
                foreach (var Obj in employeeList)
                {
                    count++;
                }
                ReturnFromLaundry laundry = new ReturnFromLaundry();
                laundry.LaundryReturnDate = DateTime.Today;
                laundry.Quantity = count;
                db.ReturnFromLaundries.Add(laundry);
                db.SaveChanges();
                foreach (var Obj in employeeList)
                {
                    var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == Obj.StockBarcode);
                    if (result != null)
                    {
                       
                        var res = db.LaundryLines.SingleOrDefault(b => b.StockID == result.StockID && b.StockStatusID == 2);
                        if (res != null)
                        {
                            ReturnLine returnLine = new ReturnLine();
                            returnLine.StockID = res.StockID;
                            returnLine.ReturnID = laundry.ReturnID;
                            returnLine.CheckInLaundryID = res.CheckInLaundryID;
                            db.ReturnLines.Add(returnLine);
                            res.StockStatusID = 1;
                            result.StockStatusID = 1;
                            db.SaveChanges();
                        
                        }

                    }
                }
                return Json("Successfully transfered to Out of the Laundry", JsonRequestBehavior.AllowGet);
            }
            else
            if (transferType == "RetailToRental")
            {
                foreach (var Obj in employeeList)
                {
                    var result = db.Stocks.SingleOrDefault(b => b.StockBarcode == Obj.StockBarcode && b.StockStatusID ==1);
                    if (result != null)
                    {
                        result.DepartmentID = 1;
                    }
                }
                return Json("Successfully transfered to Retail", JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult PrepareReport()
        {
            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/StockDescriptions.json"));
            var employeeList = JsonConvert.DeserializeObject<List<RetrieveData>>(jsonData)
                   ?? new List<RetrieveData>();
            TransferReport transferRepo = new TransferReport();
            string ImgPath = Server.MapPath("~/Reporting/LetterHead.png");
            byte[] aBytes = transferRepo.PrepareReport(employeeList, ImgPath);
          
       
            System.IO.File.WriteAllBytes(Server.MapPath("~/Reporting/Transfer.pdf"), aBytes);
            return File(aBytes, "application/pdf");
        }
        [Obsolete]
        public void generateTransferReport()
        {
            string EmployeeName = "Mr Matome Mhlangani";
            string OrderNumber =Convert.ToString(0768);
            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/StockDescriptions.json"));
            var employeeList = JsonConvert.DeserializeObject<List<RetrieveData>>(jsonData)
                   ?? new List<RetrieveData>();
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[5] {
                           new DataColumn("StockBarcode", typeof(string)),
                            new DataColumn("StockType", typeof(string)),
                            new DataColumn("StockColour", typeof(string)),
                            new DataColumn("StockSize", typeof(string)),
                            new DataColumn("Department", typeof(string))}
                            );

            foreach (var Obj in employeeList)
            {
                dt.Rows.Add(Obj.StockBarcode, Obj.StockType, Obj.StockColour, Obj.StockSize, Obj.Department);
            }

   
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Order Sheet</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Order No: </b>");
                    sb.Append(OrderNumber);
                    sb.Append("</td><td align = 'right'><b>Date: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Company Name: </b>");
                    sb.Append(EmployeeName);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");
                    sb.Append("<table border = '1'>");
                    sb.Append("<tr>");
                    foreach (DataColumn column in dt.Columns)
                    {
                        sb.Append("<th style = 'background-color: #D20B0C;color:#ffffff'>");
                        sb.Append(column.ColumnName);
                        sb.Append("</th>");
                    }
                    sb.Append("</tr>");
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("<td>");
                            sb.Append(row[column]);
                            sb.Append("</td>");
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                    StringReader sr = new StringReader(sb.ToString());
                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


               
                    
                 
                  
                    MemoryStream ms = new MemoryStream();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        htmlparser.Parse(sr);
                        pdfDoc.Close();
                        byte[] bytes = stream.ToArray();
                        System.IO.File.WriteAllBytes(Server.MapPath("~/Reporting/CustomerReport.pdf"),bytes);
                        stream.Close();
                        Response.Clear();
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("content-disposition", "attachment;filename=CustomerReport.pdf");
                        Response.Buffer = true;
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.Close();
                        Response.End();
                    }
                        
                   
                  
                }


            }

        
        }
        public ActionResult loadData()
        {
            
            var stockDescriptions = db.StockDescriptions.Include(s => s.Colour).Include(s => s.Size.SizeDescription).Include(s => s.StockType.DESCRIPTION);

            var b = (from obj in stockDescriptions select new { ID = obj.StockDescriptionID, ColourName = obj.Colour.ColourName, Size = obj.Size.SizeDescription, StockType = obj.StockType.DESCRIPTION , space=""});

            return Json(new { data = b.ToList() }, JsonRequestBehavior.AllowGet);



        }




        private string ImgName;
        private byte[] BarImg;
        [HttpGet]
      
        public ActionResult SaveStock(string ID, string Location, string Department, string StockType)
        {
            string Barcode = GenerateBarCode(StockType);
            string Imgurl = ViewBag.BarcodeImage;
            int Loca = Convert.ToInt32(Location);
            int _id = Convert.ToInt32(ID);
            int Dep = Convert.ToInt32(Department);
            int state = 1;
            int Stockstatus = 1;
            Stock stock = new Stock();
            stock.DepartmentID = Dep;
            stock.LocationID = Loca;
            stock.StockDescriptionID = _id;
            stock.BarcodeImage = BarImg;
            stock.ImageName = ImgName;
            stock.StockStateID = state;
            stock.StockBarcode = Barcode;
            stock.StockStatusID = Stockstatus;
            db.Stocks.Add(stock);
            db.SaveChanges();
            UpdateRentalDescription(_id, Dep);
            return Json(ImgName, JsonRequestBehavior.AllowGet);
        }

        public void UpdateRentalDescription(int id, int dep)
        {
            var result = db.StockDescriptions.FirstOrDefault(b => b.StockDescriptionID == id);
            if (result != null)
            {
                if (dep == 1)
                    result.RENTALSTOCKLEVEL += 1;
                else
                    result.RETAILSTOCKLEVEL += 1;

                db.SaveChanges();
            }
        
        }
        public String GenerateBarCode(string StockType)
        {

            /*first 2 letters from the type,  3 letters from the name,  type number,location ,department   eg : */
            int L = 9;
            var random = new Random();
            string s = string.Empty;
            s += StockType[0]+ StockType[2];
            for (int i = 0; i < L; i++)
                s = String.Concat(s, random.Next(1,9).ToString());
            GenerateBarCodeImg(s);
            return s;

            
        }
        public void GenerateBarCodeImg(string bar)
        {
            var barcode = bar.ToString();
            System.Web.UI.WebControls.Image imBarCode = new System.Web.UI.WebControls.Image();
            using (Bitmap bitMap = new Bitmap(barcode.Length * 40, 80))
            {
                using (Graphics graphics = Graphics.FromImage(bitMap))
                {
                    System.Drawing.Font ofont = new System.Drawing.Font("IDAutomationHC39M Free Version", 16);
                    PointF oPoint = new PointF(2f, 2f);
                    SolidBrush blackBrush = new SolidBrush(Color.Black);
                    SolidBrush whiteBrush = new SolidBrush(Color.White);
                    graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);
                    graphics.DrawString("*" + barcode + "*", ofont, blackBrush, oPoint);
                }
                using (MemoryStream ms = new MemoryStream())
                {

                    ImgName = barcode.Substring(1, 4);
                    ImgName += "barcode.png";
                    string mypath = Request.PhysicalApplicationPath + "Images\\";
                    bitMap.Save(mypath + ImgName, ImageFormat.Png);

                   // bitMap.Save(ms, ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    BarImg = byteImage;
                    Convert.ToBase64String(byteImage);
                    imBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                    //The Image is finally converted to Base64 string

                }

                ViewBag.BarcodeImage = imBarCode.ImageUrl;
            }
        }
        // GET: Stock/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // GET: Stock/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DESCRIPTION");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "DESCRIPTION");
            ViewBag.StockStatusID = new SelectList(db.Status, "StockStatusID", "StatusDescription");
            ViewBag.StockDescriptionID = new SelectList(db.StockDescriptions, "StockDescriptionID", "StockDescriptionID");
            ViewBag.StockStateID = new SelectList(db.StockStates, "StockStateID", "STATE");
            return View();
        }

        // POST: Stock/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockID,StockStatusID,StockDescriptionID,StockStateID,LocationID,DepartmentID")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Stocks.Add(stock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DESCRIPTION", stock.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "DESCRIPTION", stock.LocationID);
            ViewBag.StockStatusID = new SelectList(db.Status, "StockStatusID", "StatusDescription", stock.StockStatusID);
            ViewBag.StockDescriptionID = new SelectList(db.StockDescriptions, "StockDescriptionID", "StockDescriptionID", stock.StockDescriptionID);
            ViewBag.StockStateID = new SelectList(db.StockStates, "StockStateID", "STATE", stock.StockStateID);
            return View(stock);
        }

        // GET: Stock/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DESCRIPTION", stock.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "DESCRIPTION", stock.LocationID);
            ViewBag.StockStatusID = new SelectList(db.Status, "StockStatusID", "StatusDescription", stock.StockStatusID);
            ViewBag.StockDescriptionID = new SelectList(db.StockDescriptions, "StockDescriptionID", "StockDescriptionID", stock.StockDescriptionID);
            ViewBag.StockStateID = new SelectList(db.StockStates, "StockStateID", "STATE", stock.StockStateID);
            return View(stock);
        }

        // POST: Stock/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockID,StockStatusID,StockDescriptionID,StockStateID,LocationID,DepartmentID")] Stock stock)
        {
            if (ModelState.IsValid)
            {
               
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DESCRIPTION", stock.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "DESCRIPTION", stock.LocationID);
            ViewBag.StockStatusID = new SelectList(db.Status, "StockStatusID", "StatusDescription", stock.StockStatusID);
            ViewBag.StockDescriptionID = new SelectList(db.StockDescriptions, "StockDescriptionID", "StockDescriptionID", stock.StockDescriptionID);
            ViewBag.StockStateID = new SelectList(db.StockStates, "StockStateID", "STATE", stock.StockStateID);
            return View(stock);
        }

        // GET: Stock/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stock stock = db.Stocks.Find(id);
            db.Stocks.Remove(stock);
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
