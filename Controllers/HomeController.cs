using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using belt_retake.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace belt_retake.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Index");
            }
            var allhobbies = dbContext.Hobbies.Include(u => u.Likedby).ThenInclude(u => u.User).ToList();
            return View(allhobbies);
        }

        [HttpGet("hobby/new")]
        public IActionResult NewHobby()
        {
            if(HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet("hobby/{hobid}")]
        public IActionResult ShowHobby(int hobid)
        {
            if(HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Index");
            }
            int id = (int)HttpContext.Session.GetInt32("userid");
            var loggeduser = dbContext.Users.Include(u => u.LikedHobbies).ThenInclude(u => u.Hobby).FirstOrDefault(i => i.UserId == id);
            var thishobby = dbContext.Hobbies.Include(u => u. Likedby).ThenInclude(u => u.User).FirstOrDefault(i => i.HobbyId == hobid);
            ViewBag.User = loggeduser;
            ViewBag.Hobby = thishobby;
            return View(thishobby);
        }

        [HttpGet("hobby/edit/{hobid}")]
        public IActionResult EditHobby(int hobid)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Index");
            }
            var thishobby = dbContext.Hobbies.FirstOrDefault(u => u.HobbyId == hobid);
            ViewBag.Hobby = thishobby;
            return View();
        }

        [HttpPost("hobby/edit/edithobby/{hobid}")]
        public IActionResult UpdateHobby(int hobid, Hobby updatehobby)
        {
            var hobbytoupdate = dbContext.Hobbies.FirstOrDefault(u => u.HobbyId == hobid);
            hobbytoupdate.Name = updatehobby.Name;
            hobbytoupdate.Description = updatehobby.Description;
            hobbytoupdate.UpdatedAt = DateTime.Now;
            dbContext.SaveChanges();
            return RedirectToAction("ShowHobby", new { hobid = hobbytoupdate.HobbyId});
        }

        [HttpPost("hobby/likehobby/{hobid}")]
        public IActionResult LikeHobby(int hobid, Like newlike)
        {
            int thisuserid = (int)HttpContext.Session.GetInt32("userid");
            Hobby thishobby = dbContext.Hobbies.FirstOrDefault(u => u.HobbyId == hobid);
            User loggeduser = dbContext.Users.Include(u => u.LikedHobbies).ThenInclude(u => u.Hobby).FirstOrDefault(i => i.UserId == thisuserid);

            newlike.UserId = loggeduser.UserId;
            newlike.HobbyId = thishobby.HobbyId;
            dbContext.Likes.Add(newlike);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult CreateHobby(Hobby newhobby)
        {
            if (dbContext.Hobbies.Any(u => u.Name == newhobby.Name))
            {
                ModelState.AddModelError("Name", "A hobby with that name already exists!");
            }
            if(ModelState.IsValid)
            {
                int id = (int)HttpContext.Session.GetInt32("userid");
                newhobby.UserId = id;
                dbContext.Hobbies.Add(newhobby);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("NewHobby");
        }

        [HttpPost]
        public IActionResult Register(IndexViewModel modelData)
        {
            User newuser = modelData.Register;
            if(dbContext.Users.Any(u => u.Email == newuser.Email))
            {
                ModelState.AddModelError("Register.Email", "Email already in use!");
            }
            if(dbContext.Users.Any(u => u.Username == newuser.Username))
            {
                ModelState.AddModelError("Register.Username", "Username already in use!");
            }
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newuser.Password = Hasher.HashPassword(newuser, newuser.Password);
                dbContext.Add(newuser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("userid",newuser.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        [HttpPost]
        public IActionResult Login(IndexViewModel modelData)
        {
            LoginUser loguser = modelData.Login;
            if(ModelState.IsValid)
            {
                var userindb = dbContext.Users.FirstOrDefault(u => u.Email == loguser.Email);
                if(userindb == null)
                {
                    ModelState.AddModelError("Login.Email", "Invalid Email/Password");
                    return View("Index");
                }
                else
                {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(loguser,userindb.Password, loguser.Password);
                    if(result == 0)
                    {
                        ModelState.AddModelError("Login.Email", "Invalid Email/Password");
                        return View("Index");
                    }
                    else
                    {
                        User loggeduser = dbContext.Users.FirstOrDefault(u => u.Email == loguser.Email);
                        HttpContext.Session.SetInt32("userid",loggeduser.UserId);
                        return RedirectToAction("Dashboard");
                    }
                }
            }
            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
