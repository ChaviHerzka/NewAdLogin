using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NewAddLogin.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewAddLogin.Web.Models;

namespace NewAddLogin.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString =
        "Data Source=.\\sqlexpress;Initial Catalog=Adds;Integrated Security=true;";

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user, string password)
        {
            var db = new AdDb(_connectionString);
            db.AddUser(user, password);
            return Redirect("/");
        }
        public IActionResult Login() 
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];  
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var db = new AdDb(_connectionString);
            var user = db.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid email/password combination";
                return Redirect("/account/login");
            }

            var claims = new List<Claim>
            {
                new Claim("user", email) // this will get set to User.Identity.Name
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/newad");
        }
        public IActionResult MyAccount() 
        {
            var db = new AdDb(_connectionString);
            var email = User.Identity.Name;
            var currentuser = db.GetByEmail(email);
            HomePageViewModel vm = new HomePageViewModel();
            vm.Ads = db.MyAccount(currentuser.Id);
            return View(vm);
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
    }
}
