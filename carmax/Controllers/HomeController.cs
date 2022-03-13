using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using carmax.Models;
namespace carmax.Controllers
{
    public class HomeController : Controller
    {
        carmaxEntities1 db = new carmaxEntities1();
        public ActionResult Index()
        {
            if(Session["userType"] != null && Session["userType"].Equals("admin"))
            {
                return RedirectToAction("Index", "admin");
            }
            List<product> car = db.products.Take(3).ToList();

            return View(car);
        }
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Register()
        {
            if(Session["username"] == null)
            {
                return View("Register");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult RegisterData(string username,string email,string phone,string password)
        {
            int resCount = db.logins.Where(temp => temp.email.Equals(email)).Count();
            if(resCount>0)
            {
               
                TempData["msg"] = "The email already exist! Please log in.";
                return RedirectToAction("Login");
            }
            else
            {
                System.Security.Cryptography.MD5CryptoServiceProvider test123 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
                data = test123.ComputeHash(data);
                String newPassword = System.Text.Encoding.ASCII.GetString(data);
                login lg = new login();
                lg.username = username;
                lg.email = email;
                lg.phone = phone;
                lg.type = "normal";
                lg.password = newPassword;
                db.logins.Add(lg);
                db.SaveChanges();

               
                TempData["msg"] = "Register completed! Please log in.";
                return RedirectToAction("Login");
            }
            
        }
        [HttpPost]
        public ActionResult LoginData(string email,string password)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider test123 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = test123.ComputeHash(data);
            String newPassword = System.Text.Encoding.ASCII.GetString(data);
            int resCount = db.logins.Where(temp => temp.email.Equals(email) & temp.password.Equals(newPassword)).Count();
            if(resCount > 0)
            {
                List<login> user = db.logins.Where(temp=> temp.email.Equals(email) ).ToList();
                foreach (var i in user)
                {
                    Session["username"] = i.username;
                    Session["userType"] = i.type;
                    Session["userid"] = i.id;
                    Session["email"] = i.email;
                    Session["phone"] = i.phone;
                }
                if (Session["userType"].Equals("admin"))
                {
                    return RedirectToAction("Index","admin");
                }
                else
                {
                    return RedirectToAction("Index");
                }
                   
            }
            else
            {
                
                TempData["msg"] = "Email or Password does not match! Try again.";
                return RedirectToAction("Login");
            }
        }
        [HttpGet]
        [OutputCache(NoStore =true,Duration =0, VaryByParam ="None")]
        public ActionResult Login()
        {

            if (Session["username"] == null)
            {
                if (TempData["msg"] != null)
                {
                    ViewBag.Message = TempData["msg"].ToString();
                }
                return View("login");

            }
            else
            {
                return RedirectToAction("Index");
            }


        }

        public ActionResult Cars()
        {
            List<product> car = db.products.Where(temp => temp.type.Equals("normal")).ToList();
            return View(car);
        }
        public ActionResult SpecialOffers()
        {
            if(Session["username"] != null)
            {
                List<product> car = db.products.Where(temp => temp.type.Equals("private")).ToList();
                return View(car);
            }
            else
            {
                TempData["msg"] = "Please login to see this page";
                return RedirectToAction("Login");
            }
        }

        public ActionResult Team()
        {
            List<teammember> team = db.teammembers.ToList();
            return View(team);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult PostDetails(int _id)
        {
            if (TempData["msg33"] != null)
            {
                ViewBag.Message = TempData["msg33"].ToString();
            }
            product id = db.products.Find(_id);
            return View(id);
        }
        [HttpPost, ActionName("Order")]
        public ActionResult Order(int id, string name, double price)
        {
            if (Session["username"] == null)
            {
                if (TempData["msg"] != null)
                {
                    ViewBag.Message = TempData["msg"].ToString();
                }
                return View("login");

            }
            else
            {
                var time = DateTime.Now;
                order_cars ordr = new order_cars();
                ordr.user_id = (int)Session["userid"];
                ordr.email = (string)Session["email"];
                ordr.phone = (string)Session["phone"];
                ordr.car_id = id;
                ordr.car_name = name;
                ordr.car_price = price;
                ordr.time = time.ToString();
                ordr.details = "pending";
                db.order_cars.Add(ordr);
                db.SaveChanges();
                TempData["msg33"] = "Your order has been placed";
                return RedirectToAction("PostDetails", new { _id = ordr.car_id });
            }
        }
        [HttpPost]
        public ActionResult contactUs(string name,string email,string message)
        {
            
                contactu cn = new contactu();
                cn.name = name;
                cn.email = email;
                cn.message = message;
                db.contactus.Add(cn);
                db.SaveChanges();
                TempData["msg1"] = "Message sent!";
                return RedirectToAction("contactUs");
           
           
            
        }
        public ActionResult contactUs()
        {
            if (TempData["msg1"] != null)
            {
                ViewBag.Message = TempData["msg1"].ToString();
            }
            return View();
        }
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Profile()
        {
            if (Session["userid"] != null)
            {
                if (TempData["msg2"] != null)
                {
                    ViewBag.Message = TempData["msg2"].ToString();
                }
                login user = db.logins.Find(Session["userid"]);
                return View(user);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public ActionResult Profile(string username, string email, string phone, string cpassword, string npassword)
        {

            System.Security.Cryptography.MD5CryptoServiceProvider test123 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(cpassword);
            data = test123.ComputeHash(data);
            String cPassword = System.Text.Encoding.ASCII.GetString(data);
            string databasePass = db.logins.Where(temp => temp.email.Equals(email)).Select(o => o.password).FirstOrDefault();
            var user = db.logins.Where(s => s.email.Equals(email)).FirstOrDefault();
            if (databasePass == cPassword)
            {
                if (npassword == "")
                {
                    user.username = username;
                    user.email = email;
                    user.phone = phone;
                    user.password = cPassword;

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Profile");
                }
                else
                {

                    byte[] data1 = System.Text.Encoding.ASCII.GetBytes(npassword);
                    data1 = test123.ComputeHash(data1);
                    String nPassword = System.Text.Encoding.ASCII.GetString(data1);
                    user.username = username;
                    user.email = email;
                    user.phone = phone;
                    user.password = nPassword;

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Profile");
                }

            }
            else
            {
                TempData["msg2"] = "Password incorrect!";
                return RedirectToAction("Profile");
            }

        }
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        
        public ActionResult previousOrderedCars()
        {
            if(Session["userid"]==null)
            {
                return RedirectToAction("Login");
            }
            int id = (int)Session["userid"];
            List<order_cars> oCars = db.order_cars.Where(o=> o.user_id == id).ToList();
            return View(oCars);
        }

    }
}