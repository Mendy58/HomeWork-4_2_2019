using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PictureManagerLibrary;

namespace HomeWork_4_2_2019.Controllers
{
    public class HomeController : Controller
    {
        PicturedbManager mgr = new PicturedbManager(Properties.Settings.Default.ConStr);
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPassword(string Password, int id)
        {
            if(mgr.GetPasswords().Contains(Password))
            {
                HttpCookie FromRequest = Request.Cookies["Passwords"];
                string pwords = "";
                if (FromRequest != null)
                {
                    pwords = FromRequest.Value.ToString();
                }
                pwords += $",{Password}";
                HttpCookie cookie = new HttpCookie("Passwords", pwords);
                Response.Cookies.Add(cookie);
            }
            return Redirect($"~/Home/Image?id={id}");
        }

        public ActionResult Image(int id)
        {
            HttpCookie FromRequest = Request.Cookies["Passwords"];
            var Passwords = new List<string>();

            if (FromRequest != null)
            {
                string userids = FromRequest.Value.ToString();
                Passwords = userids.Split(',').ToList();
            }

            Picture pic = mgr.GetPictureByid(id);

            if(!Passwords.Contains(pic.Password) || Passwords == null)
            {
                pic.Name = null;
            }
            else
            {
                mgr.AddCountToPic(id);
            }

            return View(pic);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase image, string description, string Password)
        {
            string ext = Path.GetExtension(image.FileName);
            string fileName = $"{Guid.NewGuid()}{ext}";
            string fullPath = $"{Server.MapPath("/UploadedPictures")}\\{fileName}";
            image.SaveAs(fullPath);
            
            int x = mgr.AddPic(new Picture
            {
                Name = fileName,
                Description = description,
                Password = Password
            });
            return (RedirectToAction("Link", new { message = $"/Home/Image?id=" + x }));
        }
        public ActionResult Link(string message)
        {
            return View(message);
        }
    }
}