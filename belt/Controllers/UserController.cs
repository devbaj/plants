using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using belt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace belt.Controllers
{
  public class UserController : Controller
  {
    public Context dbContext;
    public UserController(Context context)
    {
      dbContext = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
      IndexForms pageForms = new IndexForms();
      pageForms.LoginForm = new Login();
      pageForms.RegisterForm = new Register();
      return View(pageForms);
    }

    [HttpPost("register")]
    public IActionResult Register(Register formData)
    {
      if (dbContext.Users.Any(u => u.Email == formData.Email))
      {
        ModelState.AddModelError("Email", "Email already in use");
      }
      if (!ModelState.IsValid)
      {
        IndexForms fresh = new IndexForms();
        fresh.LoginForm = new Login();
        fresh.RegisterForm = new Register();
        return View("Index", fresh);
      }
      var Hasher = new PasswordHasher<Register>();
      User newUser = new User();
      newUser.FirstName = formData.FirstName;
      newUser.LastName = formData.LastName;
      newUser.HashedPassword = Hasher.HashPassword(formData, formData.Password);
      newUser.Email = formData.Email;
      newUser.CreatedAt = DateTime.Now;
      newUser.UpdatedAt = DateTime.Now;
      dbContext.Users.Add(newUser);
      dbContext.SaveChanges();
      User thisUser = dbContext.Users
        .Single(u => u.Email == formData.Email);

      return RedirectToAction("UserApproved", new{userid = thisUser.UserID});
    }

    [HttpPost("login")]
    public IActionResult Login(Login formData)
    {
      User thisUser = dbContext.Users
        .SingleOrDefault(u => u.Email == formData.Email);
      if (User == null)
      {
        ModelState.AddModelError("Email", "Invalid Email or Password");
      }
      var Hasher = new PasswordHasher<Login>();
      var match = Hasher
        .VerifyHashedPassword(
          formData, thisUser.HashedPassword, formData.Password);
      if (match == 0)
      {
        ModelState.AddModelError("Email", "Invalid Email or Password");
      }
      if (!ModelState.IsValid)
      {
        IndexForms fresh = new IndexForms();
        fresh.LoginForm = new Login();
        fresh.RegisterForm = new Register();
        return View("Index", fresh);
      }
      return RedirectToAction("UserApproved", new{userid = thisUser.UserID});
    }

    public IActionResult UserApproved(int userid)
    {
      HttpContext.Session.SetInt32("userid", userid);
      return RedirectToAction("Dashboard", "Home");
    }
  }
}