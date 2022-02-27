using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using carmax.Models;
using System.IO;
using System.Net;
using System.Data.Entity;

namespace carmax.Controllers
{
    public class AdminController : Controller
    {
        carmaxEntities1 db = new carmaxEntities1();
        
        public ActionResult Index()
        {
            if (Session["userType"] != null && Session["userType"].Equals("admin"))
            {
                return View("allCars",db.products.ToList());
            }
            return Content("Please log in as Admin to use the url!");
        }
        public ActionResult Create()
        {
            if (Session["userType"] != null && Session["userType"].Equals("admin"))
            {
                return View();
            }
            return Content("Please log in as Admin to use the url!");   
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Price,Max_power,Seating_capacity,Body_type,Fuel_type,No_of_cylinder,Color,Engine_type,Engine_displacement,Type,Image,Brand,Model,Year,Discount,File")] product car)
        {
            if (ModelState.IsValid)
            {
                var filename = Path.GetFileName(car.File.FileName);
                string _filename = DateTime.Now.ToString("hhmmssfff") + filename;
                string path = Path.Combine(Server.MapPath("~/Images/"), _filename);
                car.img = "~/Images/" + _filename;

                db.products.Add(car);
                if (db.SaveChanges() > 0)
                {
                    car.File.SaveAs(path);
                }
                return RedirectToAction("Index");
            }

            return View(car);
        }

        public ActionResult Edit(int? id)
        {
            if (Session["userType"] != null && Session["userType"].Equals("admin"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                product car = db.products.Find(id);
                Session["imgPath"] = car.img;
                if (car == null)
                {
                    return HttpNotFound();
                }
                return View("Edit", car);
            }
            return Content("Please log in as Admin to use the url!");

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price,Max_power,Seating_capacity,Body_type,Fuel_type,No_of_cylinder,Color,Engine_type,Engine_displacement,Type,Image,Brand,Model,Year,Discount, File")] product car)
        {
            if (ModelState.IsValid)
            {
                if (car.File != null)
                {
                    var filename = Path.GetFileName(car.File.FileName);
                    string _filename = DateTime.Now.ToString("hhmmssfff") + filename;
                    string path = Path.Combine(Server.MapPath("~/Images/"), _filename);
                    car.img = "~/Images/" + _filename;

                    db.Entry(car).State = EntityState.Modified;
                    string oldImgPath = Request.MapPath(Session["imgPath"].ToString());
                    if (db.SaveChanges() > 0)
                    {
                        car.File.SaveAs(path);
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    car.img = Session["imgPath"].ToString();

                    db.Entry(car).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            return View("Edit",car);
        }

        public ActionResult Delete(int? id)
        {
            if (Session["userType"] != null && Session["userType"].Equals("admin"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                product car = db.products.Find(id);
                if (car == null)
                {
                    return HttpNotFound();
                }
                return View("Delete",car);
            }
            return Content("Please log in as Admin to use the url!");
            
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            product car = db.products.Find(id);
            string currentImg = Request.MapPath(car.img);
            db.products.Remove(car);
            if (db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(currentImg))
                {
                    System.IO.File.Delete(currentImg);
                }
            }
            return RedirectToAction("Index");
        }
    }
}