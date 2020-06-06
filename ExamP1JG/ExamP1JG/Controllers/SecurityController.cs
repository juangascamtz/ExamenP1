using ExamP1JG.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ExamP1JG.Controllers
{
    public class SecurityController : Controller
    {
        examenDBEntities db = new examenDBEntities();

        // GET: Security
        [AllowAnonymous]
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(tbl_usuario kullanici)
        {
            var user = db.tbl_usuario.FirstOrDefault(x => x.username == kullanici.username && x.password == kullanici.password);

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.username, false);
                return RedirectToAction("Home", "Home");

            }
            else
            {
                ViewBag.Mesaj = "Favor de revisar usuario y/o contraseña";
                return View();
            }

        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}