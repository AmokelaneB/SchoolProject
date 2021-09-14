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
    public class ColoursController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Colours
        public ActionResult Index()
        {
            return View(db.Colours.ToList());
        }

        // GET: Colours/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Colour colour = db.Colours.Find(id);
            if (colour == null)
            {
                return HttpNotFound();
            }
            return View(colour);
        }

        // GET: Colours/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Colours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ColourID,ColourName")] Colour colour)
        {
            if (ModelState.IsValid)
            {
                Colour _colour = db.Colours.Where(x => x.ColourName == colour.ColourName).FirstOrDefault();
                if (_colour == null)
                {
                    db.Colours.Add(colour);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_colour.ColourName == colour.ColourName)
                {
                    TempData["ErrorMessage"] = $"{_colour.ColourName} Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Colours.Add(colour);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }

            }

            return View(colour);
        }

        // GET: Colours/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Colour colour = db.Colours.Find(id);
            if (colour == null)
            {
                return HttpNotFound();
            }
            return View(colour);
        }

        // POST: Colours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ColourID,ColourName")] Colour colour)
        {
            Colour _colour = db.Colours.Where(x => x.ColourName == colour.ColourName).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (_colour != null)
                {
                    TempData["ErrorMessage"] = "No Changes Made";
                    return RedirectToAction("Index");
                }
                else if (_colour == null)
                {
                    db.Entry(colour).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(colour).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(colour);
        }

        // GET: Colours/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "A Faculty of this Colour Exists. Delete Terminated.";
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Colour colour = db.Colours.Find(id);
            if (colour != null)
            {
                InstitutionFaculty _institutionfaculty = db.InstitutionFaculties.Where(x => x.ColourID == colour.ColourID).FirstOrDefault();
                StockDescription stock = db.StockDescriptions.Where(x => x.ColourID == colour.ColourID).FirstOrDefault();
               if (_institutionfaculty != null)
            {
                TempData["ErrorMessage"] = "A Faculty of this Colour Exists. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (stock != null)
            {

                TempData["ErrorMessage"] = "A Stock Description of this Colour Exists. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.Colours.Remove(colour);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }
      

            }
            TempData["ErrorMessage"] = "Colour does not exist.";
            return RedirectToAction("Index");
            ;
        }

        //// POST: Colours/Delete/5
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
