using System;
using System.Collections.Generic;
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
            
            List<product> car = db.products.Take(3).ToList();

            return View(car);
        }
        public ActionResult Register()
        {
            if(Session["username"] == null)
            {
                return View("Register");
            }
            else
            {
                return Content("The url does not exist");
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
                }

                return RedirectToAction("Index");
            }
            else
            {
                
                TempData["msg"] = "Email or Password does not match! Try again.";
                return RedirectToAction("Login");
            }
        }
        public ActionResult Login()
        {

            if (Session["username"] == null)
            {
                if(TempData["msg"]!=null)
                {
                    ViewBag.Message = TempData["msg"].ToString();
                }
                return View("Login");
            }
            else
            {
                return Content("The url does not exist");
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
    }
}