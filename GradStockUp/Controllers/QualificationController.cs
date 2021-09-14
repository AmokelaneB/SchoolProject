using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace GradStockUp.Controllers
{
    public class QualificationsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Qualifications
        public ActionResult Index()
        {
            return View(db.Qualifications.ToList());
        }

        public ActionResult getColour()
        {
            return Json(db.Colours.Select(x => new
            {

                EstaID = x.ColourID,
                Description = x.ColourName,

            }).ToList(), JsonRequestBehavior.AllowGet);

        }
        public ActionResult getStockType()
        {
            var Sizes = from Establish in db.Establishments
                        from StckType in Establish.StockTypes
                        select new
                        {
                            StckTypeID = StckType.StockTypeID,
                            description = StckType.DESCRIPTION,
                            EstaID = Establish.EstablishmentID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);
        }
        // GET: Qualifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qualification qualification = db.Qualifications.Find(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // GET: Qualifications/Create
        public ActionResult Create()
        {
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/QualificationList.json"), string.Empty);
            System.IO.File.WriteAllText(Server.MapPath("~/Scripts/StockTypeList.json"), string.Empty);
            return View();
        }

        public ActionResult getInstitution()
        {

            var Sizes = from Establish in db.Establishments
                        from Institute in Establish.Institutions
                        select new
                        {
                            InstID = Institute.InstitutionID,
                            description = Institute.InstitutionName,
                            EstaID = Establish.EstablishmentID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);

        }
        public ActionResult getFaculties()
        {
            var Sizes = from Institut in db.Institutions
                        from Fac in Institut.Faculties
                        select new
                        {
                            InstID = Institut.InstitutionID,
                            description = Fac.Description,
                            FacID = Fac.FacultyID
                        };

            return Json(Sizes.ToList(), JsonRequestBehavior.AllowGet);
        
         }
        
      public void DeleteFromStockList(string stckType)
        {

            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/StockTypeList.json"));
            dynamic array = JsonConvert.DeserializeObject(jsonData);
            List<StockTypeList> list = JsonConvert.DeserializeObject<List<StockTypeList>>(jsonData)
                    ?? new List<StockTypeList>();
            List<StockTypeList> tempList = new List<StockTypeList>();
            int count = 0;
            if(array != null)
            {
                foreach (var item in array)
                {
                    if (item.stockType == stckType)
                    {
                        TempData["SuccessMessage"] = "Deleted Successfully";
                    }
                    else
                    {
                        StockTypeList stock = new StockTypeList();
                        stock.stockType = item.stockType;
                        stock.colour = item.colour;
                        tempList.Add(stock);
                    }
                    count++;

                }
                string JsonDescription = JsonConvert.SerializeObject(tempList.ToArray());
                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/StockTypeList.json"), JsonDescription);

            }
          
        }
        public void SaveToListStock(string stockType, string ColourName)
        {
            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/StockTypeList.json"));
            dynamic array = JsonConvert.DeserializeObject(jsonData);
            List<StockTypeList> list = JsonConvert.DeserializeObject<List<StockTypeList>>(jsonData)
                    ?? new List<StockTypeList>();
            Boolean flag = false;
            if (array == null)
            {
                StockTypeList stock = new StockTypeList();
                stock.stockType = stockType;
                stock.colour = ColourName;

                list.Add(stock);
                string JsonDescription = JsonConvert.SerializeObject(list.ToArray());
                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/StockTypeList.json"), JsonDescription);

            }
            else
            {
                foreach (var item in array)
                {
                    if (item.stockType == stockType)
                    {
                        flag = true;

                    }
                }

                if (!flag)
                {
                    StockTypeList stock = new StockTypeList();
                    stock.stockType = stockType;
                    stock.colour = ColourName;

                    list.Add(stock);
                    string JsonDescription = JsonConvert.SerializeObject(list.ToArray());
                    System.IO.File.WriteAllText(Server.MapPath("~/Scripts/StockTypeList.json"), JsonDescription);
                }

            }
          

            }
        public void SaveToList(List<string> data, string Univ)
        {

            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/QualificationList.json"));
            dynamic array = JsonConvert.DeserializeObject(jsonData);
            List<QualItems> list = JsonConvert.DeserializeObject<List<QualItems>>(jsonData)
                    ?? new List<QualItems>();
            List<string> d = data;
            string Varsity = Univ;
            QualItems qual = new QualItems();
            qual.University = Varsity;
            qual.fac = d;
            Boolean bflag = false;
            if (array == null)
            {
                list.Add(qual);
                string JsonDescription = JsonConvert.SerializeObject(list.ToArray());
                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/QualificationList.json"), JsonDescription);

            }
            else
            {
                foreach (var item in array)
                {

                    if (item.University == Univ)
                    {
                        bflag = true;
                    }
                }
                if (!bflag)
                {

                    list.Add(qual);
                    string JsonDescription = JsonConvert.SerializeObject(list.ToArray());
                    System.IO.File.WriteAllText(Server.MapPath("~/Scripts/QualificationList.json"), JsonDescription);

                }

            }
          
              

        }

        public void DeleteFromList(string Univ)
        {

            var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/QualificationList.json"));
            dynamic array = JsonConvert.DeserializeObject(jsonData);
            List<QualItems> list = JsonConvert.DeserializeObject<List<QualItems>>(jsonData)
                    ?? new List<QualItems>();
            List<QualItems> tempList = new List<QualItems>();
            int count = 0;

            if (array != null)
            {
                foreach (var item in array)
                {
                    if (item.University == Univ)
                    {
                        TempData["SuccessMessage"] = "Deleted Successfully";
                    }
                    else
                    {
                        QualItems items = new QualItems();
                        items.University = item.University;
                        List<string> items1 = new List<string>();

                        foreach (var b in item.fac)
                        {
                            items1.Add(JsonConvert.SerializeObject(b));
                        }
                        items.fac = items1;
                        tempList.Add(items);
                    }
                    count++;

                }
                string JsonDescription = JsonConvert.SerializeObject(tempList.ToArray());
                System.IO.File.WriteAllText(Server.MapPath("~/Scripts/QualificationList.json"), JsonDescription);

            }
     
        }
        public ActionResult AddQualification(string QualName)
        {
            var res = db.Qualifications.FirstOrDefault(b => b.QualificationName == QualName);
            if (res == null)
            {
                Qualification q = new Qualification();
                q.QualificationName = QualName;
                db.Qualifications.Add(q);
                db.SaveChanges();
                var jsonData = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/StockTypeList.json"));
                dynamic array = JsonConvert.DeserializeObject(jsonData);
                List<StockTypeList> list = JsonConvert.DeserializeObject<List<StockTypeList>>(jsonData)
                        ?? new List<StockTypeList>();
               
                if (array != null)
                {
                    foreach (var item in array)
                    {
                        StockTypeList typeList = new StockTypeList();
                        typeList.stockType =item.stockType;
                        string stk = typeList.stockType;
                        typeList.colour = item.colour;
                        string col = typeList.colour;
                        col.Trim();
                        stk.Trim();
                        var sdsd = db.StockTypes.Where(b => b.DESCRIPTION == stk).SingleOrDefault();
                        var sddd = db.Colours.Where(b => b.ColourName == col).SingleOrDefault();
                        var bbb = db.StockTypes.FirstOrDefault(b => b.DESCRIPTION == stk);
                        var ccc = db.Colours.FirstOrDefault(b => b.ColourName == col);
                        if (sdsd != null && sddd != null)
                        {
                            int CiD = sddd.ColourID;
                            int SiD = sdsd. StockTypeID;
                            QualificationStockType qualificationStockType = new QualificationStockType();
                            var r = db.QualificationStockTypes.FirstOrDefault(b => b.ColourID == CiD && b.StockTypeID == SiD && b.QualificationID == q.QualificationID);
                            if (r == null)
                            {
                                qualificationStockType.QualificationID = q.QualificationID;
                                qualificationStockType.StockTypeID = SiD;
                                qualificationStockType.ColourID = CiD;
                                db.QualificationStockTypes.Add(qualificationStockType);
                                TempData["SuccessMessage"] = "Saved Successfully";
                                db.SaveChanges();
                            }

                        }
                     }
                }

                var jsonDatac = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/QualificationList.json"));
                dynamic arrayc = JsonConvert.DeserializeObject(jsonDatac);
                List<QualItems> QuaList = JsonConvert.DeserializeObject<List<QualItems>>(jsonDatac)
                        ?? new List<QualItems>();

                if (arrayc != null)
                {
                    foreach (var item in arrayc)
                    {
                        string Uni = item.University;
                        var UnivD = db.Institutions.FirstOrDefault(b => b.InstitutionName == Uni);

                        if (UnivD != null)
                        {

                            if (item.fac = null)
                            {
                                foreach (var bac in item.fac)
                                {

                                    string Facu = bac;
                                    var Fa = db.Faculties.FirstOrDefault(b => b.Description == Facu);
                                    if (Fa != null)
                                    {

                                        var FacQual = db.FacultyQualifications.FirstOrDefault(b => b.FacultyID == Fa.FacultyID && b.InstitutionID == UnivD.InstitutionID && b.QualificationID == q.QualificationID);
                                        if (FacQual == null)
                                        {
                                            FacultyQualification faculty = new FacultyQualification();
                                            faculty.InstitutionID = UnivD.InstitutionID;
                                            faculty.QualificationID = q.QualificationID;
                                            faculty.FacultyID = Fa.FacultyID;
                                            db.FacultyQualifications.Add(faculty);
                                            db.SaveChanges();

                                        }


                                    }
                                }

                            }


                         


                        }
                    }
                 
                }
                TempData["SuccessMessage"] = "Saved Successfully";
                return Json("success", JsonRequestBehavior.AllowGet);

            }
            TempData["ErrorMessage"] = "Qualification Already Exists";
            return Json("Failed", JsonRequestBehavior.AllowGet);
        }
        // POST: Qualifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QualificationID,QualificationName")] Qualification qualification)
        {
            if (ModelState.IsValid)
            {
                Qualification _qualification = db.Qualifications.Where(x => x.QualificationName == qualification.QualificationName).FirstOrDefault();
                if (_qualification == null)
                {
                    db.Qualifications.Add(qualification);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_qualification.QualificationName == qualification.QualificationName)
                {
                    TempData["ErrorMessage"] = "Qualification Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Qualifications.Add(qualification);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(qualification);
        }

        // GET: Qualifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qualification qualification = db.Qualifications.Find(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // POST: Qualifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QualificationID,QualificationName")] Qualification qualification)
        {
            if (ModelState.IsValid)
            {
                Qualification _qualification = db.Qualifications.Where(x => x.QualificationName == qualification.QualificationName).FirstOrDefault();
                if (_qualification == null)
                {
                    db.Entry(qualification).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_qualification != null)
                {
                    TempData["ErrorMessage"] = "No Changes Made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(qualification).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(qualification);
        }

        // GET: Qualifications/Delete/5
        public ActionResult Delete(int? id)
        {
            Qualification qualification = db.Qualifications.Find(id);
            //var quaT = from qualificationT in db.QualificationTypes
            //          from Qua in qualificationT.Qualifications
            //          select new
            //          {
            //              QuaType = Qua.QualificationTypeID,

            //          };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var res = db.QualificationStockTypes.FirstOrDefault(b => b.QualificationID == id);

            if (res == null)
            {

                db.Qualifications.Remove(qualification);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Types exits of this Qualification. Delete Terminated.";
               return RedirectToAction("Index");

            }
           
            //else if (quaT != null)
            //{
            //    TempData["ErrorMessage"] = "Types exits of this Qualification. Delete Terminated.";
            //    return RedirectToAction("Index");
            //}
    

        }

        //// POST: Qualifications/Delete/5
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
        public ActionResult CreateQualificationQualificationType()
        {
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "QualificationName");
            ViewBag.QualificationTypes = db.QualificationTypes;

            return View();
        }
        [HttpPost]
        public ActionResult CreateQualificationQualificationType(int[] QualificationTypeIDs, int ? QualificationID)
        {
            if (QualificationTypeIDs != null || QualificationID !=null)
            {
                for (int i = 0; i < QualificationTypeIDs.Length; i++)
                {
                    var temp = QualificationTypeIDs[i];
                    QualificationType quaT_ = db.QualificationTypes.FirstOrDefault(x => x.QualificationTypeID == temp);
                    db.Qualifications.Where(q => q.QualificationID == QualificationID).FirstOrDefault().QualificationTypes.Add(quaT_);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Added-On Successfully. See Details";

                }
            }
            else if (QualificationID == null)
            {
                TempData["ErrorMessage"] = "Qualfication to Add-On to Not Selected";
            }
            else
            {
                TempData["ErrorMessage"] = "Qualfication Types to Add-On Not Selected";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
