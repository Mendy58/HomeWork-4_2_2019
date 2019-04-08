using PictureManagerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HomeWork_4_2_2019.Controllers
{
    public class UserController : Controller
    {
        PicturedbManager mgr = new PicturedbManager(Properties.Settings.Default.ConStr);

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult MyUploads()
        {
            User s = mgr.GetByEmail(User.Identity.Name);
            List<Picture> p = mgr.GetPictureByUserid(s.id);
            return View(p);
        }

        public ActionResult login(string Message)
        {
            return View(Message);
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User user, string password)
        {
            mgr.AddUser(user, password);
            return Redirect("/");
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = mgr.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid login, Try again you Irish drunk!";
                return Redirect("/User/login");
            }

            FormsAuthentication.SetAuthCookie(email, true);
            return Redirect("/");
        }

    }
}