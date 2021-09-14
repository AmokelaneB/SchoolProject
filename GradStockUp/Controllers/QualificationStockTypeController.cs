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
    public class QualificationStockTypeController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: QualificationStockType
        public ActionResult Index()
        {
            var qualificationStockTypes = db.QualificationStockTypes.Include(q => q.Colour).Include(q => q.Qualification).Include(q => q.StockType);
            return View(qualificationStockTypes.ToList());
        }

        // GET: QualificationStockType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualificationStockType qualificationStockType = db.QualificationStockTypes.Find(id);
            if (qualificationStockType == null)
            {
                return HttpNotFound();
            }
            return View(qualificationStockType);
        }

        // GET: QualificationStockType/Create
        public ActionResult Create()
        {
            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName");
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "QualificationName");
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION");
            return View();
        }

        // POST: QualificationStockType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QualificationID,ColourID,StockTypeID")] QualificationStockType qualificationStockType)
        {
            if (ModelState.IsValid)
            {
                db.QualificationStockTypes.Add(qualificationStockType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName", qualificationStockType.ColourID);
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "QualificationName", qualificationStockType.QualificationID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", qualificationStockType.StockTypeID);
            return View(qualificationStockType);
        }

        // GET: QualificationStockType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualificationStockType qualificationStockType = db.QualificationStockTypes.Find(id);
            if (qualificationStockType == null)
            {
                return HttpNotFound();
            }
            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName", qualificationStockType.ColourID);
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "QualificationName", qualificationStockType.QualificationID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", qualificationStockType.StockTypeID);
            return View(qualificationStockType);
        }

        // POST: QualificationStockType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QualificationID,ColourID,StockTypeID")] QualificationStockType qualificationStockType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qualificationStockType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName", qualificationStockType.ColourID);
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "QualificationName", qualificationStockType.QualificationID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", qualificationStockType.StockTypeID);
            return View(qualificationStockType);
        }

        // GET: QualificationStockType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualificationStockType qualificationStockType = db.QualificationStockTypes.Find(id);
            if (qualificationStockType == null)
            {
                return HttpNotFound();
            }
            return View(qualificationStockType);
        }

        // POST: QualificationStockType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QualificationStockType qualificationStockType = db.QualificationStockTypes.Find(id);
            db.QualificationStockTypes.Remove(qualificationStockType);
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
