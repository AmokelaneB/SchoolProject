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
    public class SupplierController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Suppliers
        public ActionResult Index()
        {
            return View(db.Suppliers.ToList());
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierID,SupplierName,ADDRESS,PHONENUMBER,EMAIL")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                Supplier _supplier = db.Suppliers.Where(x => x.SupplierName == supplier.SupplierName).FirstOrDefault();
                if (_supplier == null)
                {
                    db.Suppliers.Add(supplier);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_supplier.SupplierName == supplier.SupplierName)
                {
                    TempData["ErrorMessage"] = "Supplier Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Suppliers.Add(supplier);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                };
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SupplierID,SupplierName,ADDRESS,PHONENUMBER,EMAIL")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                Supplier _supplier = db.Suppliers.Where(x => x.SupplierName == supplier.SupplierName && x.ADDRESS == supplier.ADDRESS && x.PHONENUMBER == supplier.PHONENUMBER && x.EMAIL == supplier.EMAIL).FirstOrDefault();
                if (_supplier == null)
                {
                    db.Entry(supplier).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_supplier != null)
                {
                    TempData["ErrorMessage"] = "No Changes Made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(supplier).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");

                }
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            Supplier supplier = db.Suppliers.Find(id);
            SupplierOrder _supplierOrder = db.SupplierOrders.Where(x => x.SupplierID == supplier.SupplierID).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_supplierOrder != null)
            {
                TempData["ErrorMessage"] = "An Order has been made to this Supplier. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.Suppliers.Remove(supplier);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: Suppliers/Delete/5
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
