using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;

namespace GradStockUp.Controllers
{
    public class OrganizationsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Organizations
        public ActionResult Index()
        {
            return View(db.Organizations.ToList());
        }

        // GET: Organizations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // GET: Organizations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Organizations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrganizationID,Name,Address,Contact,RegistrationNO")] Organization organization)
        {
            if (ModelState.IsValid)
            {
                Organization _organization = db.Organizations.Where(x => x.Name == organization.Name && x.RegistrationNO == organization.RegistrationNO ).FirstOrDefault();
                if (_organization == null)
                {
                    db.Organizations.Add(organization);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_organization != null)
                {
                    TempData["ErrorMessage"] = "Organization Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Organizations.Add(organization);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(organization);
        }

        // GET: Organizations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: Organizations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrganizationID,Name,Address,Contact,RegistrationNO")] Organization organization)
        {
            if (ModelState.IsValid)
            {
                Organization _organization = db.Organizations.Where(x => x.Name == organization.Name && x.RegistrationNO == organization.RegistrationNO && x.Address == organization.Address && x.Contact ==organization.Contact).FirstOrDefault();
                if (_organization == null)
                {
                    db.Entry(organization).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_organization != null)
                {
                    TempData["ErrorMessage"] = "No Changes Made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(organization).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(organization);
        }

        // GET: Organizations/Delete/5
        public ActionResult Delete(int? id)
        {
            Organization organization = db.Organizations.Find(id);
            Customer _customer = db.Customers.Where(x => x.OrganizationID == organization.OrganizationID).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_customer != null)
            {
                TempData["ErrorMessage"] = "Organization Not Deleted. A customer is registered to it.";
                return RedirectToAction("Index");
            }
            else
            {
                db.Organizations.Remove(organization);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }
        }

        //// POST: Organizations/Delete/5
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
