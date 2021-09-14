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
    public class BanksController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Banks
        public ActionResult Index()
        {
            return View(db.Banks.ToList());
        }

        // GET: Banks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank bank = db.Banks.Find(id);
            if (bank == null)
            {
                return HttpNotFound();
            }
            return View(bank);
        }

        // GET: Banks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Banks/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BankID,BankName,BranchCode")] Bank bank)
        {
            if (ModelState.IsValid)
            {
                Bank _bank = db.Banks.Where(x => x.BankName == bank.BankName).FirstOrDefault();
                if (_bank == null)
                {
                    db.Banks.Add(bank);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_bank.BankName == bank.BankName)
                {
                    TempData["ErrorMessage"] = $"{_bank.BankName} Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Banks.Add(bank);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }

            }

            return View(bank);
        }

        // GET: Banks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank bank = db.Banks.Find(id);
            if (bank == null)
            {
                return HttpNotFound();
            }
            return View(bank);
        }

        // POST: Banks/Edit/5
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BankID,BankName,BranchCode")] Bank bank)
        {
            if (ModelState.IsValid)
            {
                Bank _bank = db.Banks.Where(x => x.BankName == bank.BankName).FirstOrDefault();
                if (_bank == null)
                {
                    db.Entry(bank).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_bank.BankName == bank.BankName)
                {
                    TempData["ErrorMessage"] = "No changes were made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(bank).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }

            }
            return View(bank);
        }

        // GET: Banks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Bank bank = db.Banks.Find(id);
                db.Banks.Remove(bank);
                TempData["SuccessMessage"] = "Deleted Successfully";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }

        //// POST: Banks/Delete/5
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
