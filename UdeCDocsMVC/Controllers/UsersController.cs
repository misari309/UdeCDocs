using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UdeCDocsMVC.Models;
using UdeCDocsMVC.Models.SysModels;

namespace UdeCDocsMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly UdeCDocsContext _context;

        public UsersController(UdeCDocsContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var user = _context.Users.Where(u => u.Idrol == 2);
            return View(await user.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Iduser == id);
            user.Documents = await _context.Documents.Where(d => d.Iduser == user.Iduser).ToListAsync();
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Idfaculty"] = new SelectList(_context.Faculties, "Idfaculty", "Idfaculty", user.Idfaculty);
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", user.Idrol);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Iduser,Name,Email,Institution,City,Password,Idrol,Idfaculty")] User user)
        {
            if (id != user.Iduser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Iduser))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idfaculty"] = new SelectList(_context.Faculties, "Idfaculty", "Idfaculty", user.Idfaculty);
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", user.Idrol);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.IdfacultyNavigation)
                .Include(u => u.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Iduser == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'UdeCDocsContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //SignUp
        public IActionResult SignUpUser()
        {
            return View();
        }
        //SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpUser([Bind("Name,Email,City,Password")] CUser cUser)
        {
            if (ModelState.IsValid & !_context.Users.Any(u => u.Email == cUser.Email))
            {
                User user = new User
                {
                    Name = cUser.Name,
                    Email = cUser.Email,
                    City = cUser.City,
                    Password = cUser.Password,
                    Idrol = 2
                };
                _context.Add(user);
                await _context.SaveChangesAsync();
                ViewData["Message"] = "Usuario registrado correctamente.";
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                Claim claimUserName = new Claim(ClaimTypes.Name, user.Name);
                Claim claimRole = new Claim(ClaimTypes.Role, user.Idrol.ToString());
                Claim claimIdUser = new Claim("Iduser", user.Iduser.ToString());
                Claim claimEmail = new Claim("Email", user.Email);

                identity.AddClaim(claimUserName);
                identity.AddClaim(claimRole);
                identity.AddClaim(claimIdUser);
                identity.AddClaim(claimEmail);

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.Now.AddMinutes(45)
                });

                return RedirectToAction("Index", "Home");
            }
            ViewData["Message"] = "El usuario ya está registrado.";

            return View(cUser);
        }

        //SignUp
        public IActionResult Login()
        {
            return View();
        }
        //Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email, Password")] CUser cUser)
        {
            var user = _context.Users.Where(u => u.Email == cUser.Email && u.Password == cUser.Password).Single();
            if (!_context.Users.Any(u => u.Email == cUser.Email & u.Password == cUser.Password))
            {
                ViewData["Message"] = "Usuario no registrado.";
                return View();
            }
            else 
            {
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                Claim claimUserName = new Claim(ClaimTypes.Name, user.Name);
                Claim claimRole = new Claim(ClaimTypes.Role, user.Idrol.ToString());
                Claim claimIdUsuario = new Claim("Iduser", user.Iduser.ToString());
                Claim claimEmail = new Claim("Email", user.Email);

                identity.AddClaim(claimUserName);
                identity.AddClaim(claimRole);
                identity.AddClaim(claimIdUsuario);
                identity.AddClaim(claimEmail);

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.Now.AddMinutes(45)
                });

                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        //SignUpUdeC
        public IActionResult SignUpUdeC()
        {
            ViewData["Idfaculty"] = new SelectList(_context.Faculties, "Idfaculty", "Idfaculty");
            return View();
        }
        //SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpUdeC([Bind("Name,Email,Institution,City,Idfaculty,Password")] CUserUdeC cUser)
        {
            User user = new User
            {
                Name = cUser.Name,
                Email = cUser.Email,
                Institution = cUser.Institution,
                City = cUser.City,
                Idfaculty = cUser.Idfaculty,
                Password = cUser.Password,
                Idrol = 1
            };

            if (ModelState.IsValid & !_context.Users.Any(u => u.Email == cUser.Email))
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                ViewData["Message"] = "Usuario registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idfaculty"] = new SelectList(_context.Faculties, "Idfaculty", "Idfaculty", user.Idfaculty);
            ViewData["Message"] = "El usuario ya está registrado.";
            return View(user);

        }

        private bool UserExists(int id)
        {
          return _context.Users.Any(e => e.Iduser == id);
        }
    }
}
