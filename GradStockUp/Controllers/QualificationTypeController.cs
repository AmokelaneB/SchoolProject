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
    public class QualificationTypesController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: QualificationTypes
        public ActionResult Index()
        {
            return View(db.QualificationTypes.ToList());
        }

        // GET: QualificationTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualificationType qualificationType = db.QualificationTypes.Find(id);
            if (qualificationType == null)
            {
                return HttpNotFound();
            }
            return View(qualificationType);
        }

        // GET: QualificationTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QualificationTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QualificationTypeID,QualificationType1")] QualificationType qualificationType)
        {
            if (ModelState.IsValid)
            {
                QualificationType _qualificationType = db.QualificationTypes.Where(x => x.QualificationTypeID == qualificationType.QualificationTypeID).FirstOrDefault();

                if (_qualificationType == null)
                {
                    db.QualificationTypes.Add(qualificationType);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_qualificationType.QualificationType1 == qualificationType.QualificationType1)
                {
                    TempData["ErrorMessage"] = "Qualification Type Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.QualificationTypes.Add(qualificationType);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(qualificationType);
        }

        // GET: QualificationTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualificationType qualificationType = db.QualificationTypes.Find(id);
            if (qualificationType == null)
            {
                return HttpNotFound();
            }
            return View(qualificationType);
        }

        // POST: QualificationTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QualificationTypeID,QualificationType1")] QualificationType qualificationType)
        {
            if (ModelState.IsValid)
            {
                QualificationType _qualificationType = db.QualificationTypes.Where(x => x.QualificationType1 == qualificationType.QualificationType1).FirstOrDefault();
                if (_qualificationType == null)
                {
                    db.Entry(qualificationType).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_qualificationType.QualificationType1 == qualificationType.QualificationType1)
                {
                    TempData["ErrorMessage"] = "No changes were made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(qualificationType).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(qualificationType);
        }

        // GET: QualificationTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            QualificationType qualificationType = db.QualificationTypes.Find(id);

            
            var qua = from qualification in db.Qualifications
                             from QuaT in qualification.QualificationTypes
                             select new
                             {
                                 QuaType = QuaT.QualificationTypeID,

                             };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (qua != null)
            {
                TempData["ErrorMessage"] = "There is a Qualification of this Type. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.QualificationTypes.Remove(qualificationType);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }
        }

        //// POST: QualificationTypes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    QualificationType qualificationType = db.QualificationTypes.Find(id);
        //    db.QualificationTypes.Remove(qualificationType);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult CreateQualificationQType()
        {
            ViewBag.QualificationTypeID = new SelectList(db.QualificationTypes, "QualificationTypeID", "QualificationType1");
            ViewBag.Qualifications = db.Qualifications;

            return View();
        }
        [HttpPost]
        public ActionResult CreateQualificationQType(int? QualificationTypeID, int[] QualificationIDs)
        {
            if (QualificationIDs != null)
            {
                for (int i = 0; i < QualificationIDs.Length; i++)
                {
                    var temp = QualificationIDs[i];
                    Qualification qua_ = db.Qualifications.FirstOrDefault(x => x.QualificationID == temp);
                    db.QualificationTypes.Where(q => q.QualificationTypeID == QualificationTypeID).FirstOrDefault().Qualifications.Add(qua_);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Added-On Successfully. See Details";

                }
            }
            else if (QualificationTypeID == null)
            {
                TempData["ErrorMessage"] = "Qualfication Type to Add-On to Not Selected";
            }
            else if (QualificationIDs == null)
            {
                TempData["ErrorMessage"] = "Qualfications Add-On Not Selected";
            }
            else
            {
                TempData["ErrorMessage"] = "Qualfications to Add-On Not Selected";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        public ActionResult CreateNewQualificationQType()
        {
            ViewBag.Qualifications = db.Qualifications;

            return View();
        }
        [HttpPost]
        public ActionResult CreateNewQualificationQType( string QualificationTypeName, int[] QualificationIDs)
        {
            if (QualificationIDs != null && QualificationTypeName != "")

            {
                QualificationType _qty = db.QualificationTypes.Where(x => x.QualificationType1 == QualificationTypeName).FirstOrDefault();
                if (_qty == null)
                {
                    QualificationType qu = new QualificationType();
                    qu.QualificationType1 = QualificationTypeName;
                    db.QualificationTypes.Add(qu);
                    db.SaveChanges();


                    for (int i = 0; i < QualificationIDs.Length; i++)
                    {
                        var temp = QualificationIDs[i];
                        Qualification qua_ = db.Qualifications.FirstOrDefault(x => x.QualificationID == temp);
                        db.QualificationTypes.Where(q => q.QualificationTypeID == qu.QualificationTypeID).FirstOrDefault().Qualifications.Add(qua_);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Added-On Successfully. See Details";

                    }
                }

            }
            else
            {
                TempData["ErrorMessage"] = "Please enter all the required information to add a Qualification Type";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
