using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;
using WebGrease;

namespace GradStockUp.Controllers
{
    public class CustomersController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Customers
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.Organization);
            return View(customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            ViewBag.OrganizationID = new SelectList(db.Organizations, "OrganizationID", "Name");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,OrganizationID,CustomerName,CustomerSurname,CustAddress,IDnumber,Email,PhoneNumber,NextoFKin,kinPhone")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                Customer _customer = db.Customers.Where(x => x.IDnumber == customer.IDnumber).FirstOrDefault();
                if (_customer == null)
                {
                    db.Customers.Add(customer);

                    db.SaveChanges();
   
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_customer.IDnumber == customer.IDnumber)
                {
                    TempData["ErrorMessage"] = $"{_customer.CustomerName} {_customer.CustomerSurname} Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Customers.Add(customer);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }



            }

            ViewBag.OrganizationID = new SelectList(db.Organizations, "OrganizationID", "Name", customer.Organization);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrganizationID = new SelectList(db.Organizations, "OrganizationID", "Name", customer.OrganizationID);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,OrganizationID,CustomerName,CustomerSurname,CustAddress,IDnumber,Email,PhoneNumber,NextoFKin,kinPhone")] Customer customer)
        {
            Customer _customer = db.Customers.Where(x => x.IDnumber == customer.IDnumber && x.CustomerSurname == customer.CustomerSurname && x.CustomerName == customer.CustomerName && x.Email == customer.Email
            && x.PhoneNumber == customer.PhoneNumber && x.NextoFKin == customer.NextoFKin
            && x.kinPhone == customer.kinPhone && x.OrganizationID == customer.OrganizationID).FirstOrDefault();
            Customer _custVal = db.Customers.Where(x => x.IDnumber == customer.IDnumber).FirstOrDefault();

            if (ModelState.IsValid)
            {

                if (_customer == null && _custVal == null)
                {
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"{customer.CustomerName}{customer.CustomerSurname} info Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_customer != null)
                {

                    TempData["ErrorMessage"] = "No Changes Made";
                    return RedirectToAction("Index");
                }
                else if (_custVal != null)
                {
                    TempData["ErrorMessage"] = $"No Changes Made. Customer {_custVal.CustomerName} {_custVal.CustomerSurname} with same ID Number Exists.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"{customer.CustomerName} {customer.CustomerSurname} Updated Successfully";
                    return RedirectToAction("Index");
                }

            }
            ViewBag.OrganizationID = new SelectList(db.Organizations, "OrganizationID", "Name", customer.Organization);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Customer customer = db.Customers.Find(id);
                db.Customers.Remove(customer);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: Customers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{

        //}

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
