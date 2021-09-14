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
    public class DeliveryStatusController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: DeliveryStatus
        public ActionResult Index()
        {
            return View(db.DeliveryStatus.ToList());
        }

        // GET: DeliveryStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryStatu deliveryStatu = db.DeliveryStatus.Find(id);
            if (deliveryStatu == null)
            {
                return HttpNotFound();
            }
            return View(deliveryStatu);
        }

        // GET: DeliveryStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeliveryStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeliveryStatusID,DESCRIPTION")] DeliveryStatu deliveryStatu)
        {
            if (ModelState.IsValid)
            {
                DeliveryStatu _deliverystus = db.DeliveryStatus.Where(x => x.DESCRIPTION == deliveryStatu.DESCRIPTION).FirstOrDefault();
                if (_deliverystus == null)
                {
                    db.DeliveryStatus.Add(deliveryStatu);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_deliverystus.DESCRIPTION == deliveryStatu.DESCRIPTION)
                {
                    TempData["ErrorMessage"] = $"The Status {_deliverystus.DESCRIPTION} Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.DeliveryStatus.Add(deliveryStatu);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }

            }

            return View(deliveryStatu);
        }

        // GET: DeliveryStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryStatu deliveryStatu = db.DeliveryStatus.Find(id);
            if (deliveryStatu == null)
            {
                return HttpNotFound();
            }
            return View(deliveryStatu);
        }

        // POST: DeliveryStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeliveryStatusID,DESCRIPTION")] DeliveryStatu deliveryStatu)
        {
            if (ModelState.IsValid)
            {
                DeliveryStatu _deliverystatus = db.DeliveryStatus.Where(x => x.DESCRIPTION == deliveryStatu.DESCRIPTION).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    if (_deliverystatus != null)
                    {
                        TempData["ErrorMessage"] = "No Changes Made";
                        return RedirectToAction("Index");
                    }
                    else if (_deliverystatus == null)
                    {
                        db.Entry(deliveryStatu).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Updated Successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        db.Entry(deliveryStatu).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Updated Successfully";
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(deliveryStatu);
        }

        // GET: DeliveryStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            DeliveryStatu deliveryStatus = db.DeliveryStatus.Find(id);
            Delivery _delivery = db.Deliveries.Where(x => x.DeliveryStatusID == deliveryStatus.DeliveryStatusID).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_delivery != null)
            {
                TempData["ErrorMessage"] = "A Delivery Has this Status. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.DeliveryStatus.Remove(deliveryStatus);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: DeliveryStatus/Delete/5
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
