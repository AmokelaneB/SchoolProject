using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using GradStockUp.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;
using Twilio.Types;

namespace GradStockUp.Controllers
{
    public class EmployeeController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Employee
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.EmployeeType);
            return View(employees.ToList());
        }

        // GET: Employee/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
        [HttpPost]
        public ActionResult SaveData(Employee item)
        {

            
                /*string fileName = Path.GetFileNameWithoutExtension(item.ImageUpload.FileName);
                string extension = Path.GetExtension(item.ImageUpload.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                item.PicUrl = fileName;
                item.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Images"), fileName));*/
              
            item.EmplyID = generateEmplyID();
            db.Employees.Add(item);
            db.SaveChanges();
            var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
            var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
            var fromNumber = ConfigurationManager.AppSettings["SMSAccountFrom"];

            TwilioClient.Init(accountSid, authToken);
            string Message = " Congratulations!! Please use the EmployeeID to register an Account EmployeeID:" + item.EmplyID;
            MessageResource result = MessageResource.Create(
            new PhoneNumber(item.PhoneNumber),
            from: new PhoneNumber(fromNumber),
            body: Message
            );

            ////Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            Trace.TraceInformation(result.Status.ToString());
    
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: Employee/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeTypeID = new SelectList(db.EmployeeTypes, "EmployeeTypeID", "DESCRIPTION");
            return View();
        }
        public ActionResult UploadFiles(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //Method 2 Get file details from HttpPostedFileBase class    

                    if (file != null)
                    {
                        string path = Path.Combine(Server.MapPath("~/UploadedFiles"), Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                    }
                    ViewBag.FileStatus = "File uploaded successfully.";
                }
                catch (Exception)
                {
                    ViewBag.FileStatus = "Error while file uploading."; ;
                }
            }
            return View("Index");
        }
        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        public string generateEmplyID()
        {
            int L = 9;
            var random = new Random();
            string s = string.Empty;
           
            for (int i = 0; i < L; i++)
                s = String.Concat(s, random.Next(1, 9).ToString());

            return s;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,EmployeeTypeID,Name,Surname,Email,EmployeeAddress,PhoneNumber")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.EmplyID = generateEmplyID();
                db.Employees.Add(employee);
                db.SaveChanges();
                var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
                var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
                var fromNumber = ConfigurationManager.AppSettings["SMSAccountFrom"];

                TwilioClient.Init(accountSid, authToken);
                string Message = " Congratulations!! Please use the EmployeeID to register an Account EmployeeID:" + employee.EmplyID;
                MessageResource result = MessageResource.Create(
                new PhoneNumber(employee.PhoneNumber),
                from: new PhoneNumber(fromNumber),
                body: Message
                );

                ////Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
                Trace.TraceInformation(result.Status.ToString());
     
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeTypeID = new SelectList(db.EmployeeTypes, "EmployeeTypeID", "DESCRIPTION", employee.EmployeeTypeID);
            return View(employee);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeTypeID = new SelectList(db.EmployeeTypes, "EmployeeTypeID", "DESCRIPTION", employee.EmployeeTypeID);
            return View(employee);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,EmplyID,EmployeeTypeID,Name,Surname,Email,EmployeeAddress,PhoneNumber,EmployeeImage")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeTypeID = new SelectList(db.EmployeeTypes, "EmployeeTypeID", "DESCRIPTION", employee.EmployeeTypeID);
            return View(employee);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
