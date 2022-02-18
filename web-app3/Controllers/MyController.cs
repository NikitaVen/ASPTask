using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using web_app3.Models;

namespace web_app3.Controllers
{
    public class MyController : Controller
    {

        private UserContext _db;
        public MyController(UserContext db)
        {
            this._db = db;
        }

        [Authorize]
        public async Task<IActionResult> UsersPage()
        {
           if (!(await CheckActivness(User.Identity.Name)))
                return await Logout();
            List<User> users = await _db.Users.ToListAsync();
            ViewBag.Users = users;
            return View(new EnablingUsers(users));
        }



        public async Task<IActionResult> Login()
        {
           if ( !(await CheckActivness(User.Identity.Name)))
                return await Logout();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    if (user.Status == status.Block)
                    {
                        ModelState.AddModelError("", "This account is blocked");
                        return View(model);
                    }
                    await Authenticate(model.Email);

                    user.LastLoginTime = DateTime.Now;
                    await _db.SaveChangesAsync();

                    return RedirectToAction("UsersPage");
                }
                ModelState.AddModelError("", "Incorrect Name or(and) Password");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(EnablingUsers enablingUsers)
        {
            for (int i = 0; i < enablingUsers._Checkboxes.Count; ++i)
            {
                if (enablingUsers._Checkboxes[i])
                    (await _db.Users.FirstOrDefaultAsync(x => x.ID == enablingUsers._IDs[i])).Status = status.Active;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("UsersPage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Block(EnablingUsers enablingUsers)
        {
            bool to_logout = false;
            for (int i = 0; i < enablingUsers._Checkboxes.Count; ++i)
            {
                User user_to_block;
                if (enablingUsers._Checkboxes[i])
                {
                    user_to_block = await _db.Users.FirstOrDefaultAsync(x => x.ID == enablingUsers._IDs[i]);
                    user_to_block.Status = status.Block;
                    if (User.Identity.Name == user_to_block.Email)
                        to_logout = true;
                }
            }
            await _db.SaveChangesAsync();
            if (to_logout)
                await HttpContext.SignOutAsync();
            return RedirectToAction("UsersPage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(EnablingUsers enablingUsers)
        {
            bool to_logout = false;
            for (int i = 0; i < enablingUsers._Checkboxes.Count; ++i)
            {
                User user_to_delete;
                if (enablingUsers._Checkboxes[i])
                {
                    user_to_delete = await _db.Users.FirstOrDefaultAsync(x => x.ID == enablingUsers._IDs[i]);
                    if (User.Identity.Name == user_to_delete.Email)
                        to_logout = true;
                    _db.Users.Remove(user_to_delete);
                }
            }
            await _db.SaveChangesAsync();
            if (to_logout)
                await HttpContext.SignOutAsync();
            return RedirectToAction("UsersPage");

        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public async Task<bool> CheckActivness(String email)
        {
            if (email == null)
                return true;
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email && x.Status == status.Active);
            if (user != null)
                return true;
            return false;
        }

        public async Task<IActionResult> Register()
        {
            if (!(await CheckActivness(User.Identity.Name)))
                return await Logout();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {

                    await Authenticate(model.Email);

                    _db.Users.Add(new User {
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                        Status = status.Active,
                        LastLoginTime = DateTime.Now,
                        RegistrationTime = DateTime.Now
                    });
                    await _db.SaveChangesAsync();


                    return RedirectToAction("UsersPage");
                }
                else
                    ModelState.AddModelError("", "This Email has already been registered");
            }
            return View(model);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
