using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.Auth.AccessControlPolicy;
using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UdeCDocsMVC.Models;
using UdeCDocsMVC.Models.SysModels;
using UdeCDocsMVC.Utilities;
using unirest_net.http;

namespace UdeCDocsMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly UdecDocsContext _context;

        public UsersController(UdecDocsContext context)
        {
            _context = context;
        }

        //Get user details
        // GET: Users/Details/5
        [Authorize(Policy = "RequireRegistered")]
        public async Task<IActionResult> Details(int? id)
        {
            if (User.Identity.IsAuthenticated == true)
            {
                int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);
                if (Iduser == id)
                {
                    if (id == null || _context.Users == null)
                    {
                        return NotFound();
                    }

                    var user = await _context.Users.FirstOrDefaultAsync(m => m.Iduser == id);
                    user.Documents = await _context.Documents.Where(d => d.Iduser == user.Iduser).ToListAsync();
                    user.Votes = await _context.Votes.Where(v => v.Iduser == user.Iduser).ToListAsync();
                    user.Comments = await _context.Comments.Where(c => c.Iduser == user.Iduser).ToListAsync();
                    if (user == null)
                    {
                        return NotFound();
                    }
                    return View(user);
                }

                return RedirectToAction("Index", "Home");

            }

            return RedirectToAction("Index", "Home");

        }
        //Edit user
        // GET: Users/Edit/5
        [Authorize(Policy = "RequireRegistered")]
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
            ViewData["Idfaculty"] = new SelectList(_context.Faculties, "Idfaculty", "Faculty1", user.Idfaculty);
            return View(user);
        }

        //Edit user
        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireRegistered")]
        public async Task<IActionResult> Edit(string Name, string Institution, string City)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);

            var user1 = await _context.Users.FindAsync(Iduser);


            if (Name == null)
            {
                ViewData["Idfaculty"] = new SelectList(_context.Faculties, "Idfaculty", "Faculty1", user1.Idfaculty);
                return View(user1);
            }
            else
            {
                user1.Name = Name;
                user1.Institution = Institution;
                user1.City = City;
                _context.Update(user1);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = Iduser });
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
            //API Validate Email
            string urapi = "https://mailcheck.p.rapidapi.com/?domain=" + cUser.Email;
            HttpResponse<string> response = Unirest.get(urapi).header("X-RapidAPI-Key", "9069330d76msh87f2fd09f59e5fap1893aejsnc49d292c162c")
                                                             .header("X-RapidAPI-Host", "mailcheck.p.rapidapi.com").asJson<string>();
            var body = response.Body.ToString();
            //API end

            if (ModelState.IsValid & !_context.Users.Any(u => u.Email == cUser.Email) && body.Contains("\"valid\":true,"))
            {
                Encrypt encrypt = new Encrypt();
                User user = new User
                {
                    Name = cUser.Name,
                    Email = cUser.Email,
                    City = cUser.City,
                    Password = encrypt.GetSHA256(cUser.Password),
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
            else
            {
                ViewData["Message"] = "Ingrese credenciales correctas.";
                return View(cUser);
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
            Encrypt encrypt = new Encrypt();
            var password = encrypt.GetSHA256(cUser.Password);

            if (!_context.Users.Any(u => u.Email == cUser.Email & u.Password == password))
            {
                ViewData["Message"] = "Usuario no registrado.";
                return View();
            }
            else
            {
                var user = _context.Users.Where(u => u.Email == cUser.Email && u.Password == password).Single();
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
            ViewData["Idfaculty"] = new SelectList(_context.Faculties, "Idfaculty", "Faculty1");
            return View();
        }
        //SignUpUdeC
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpUdeC([Bind("Name,Email,Institution,City,Idfaculty,Password")] CUserUdeC cUser)
        {
            Encrypt encrypt = new Encrypt();
            User user = new User
            {
                Name = cUser.Name,
                Email = cUser.Email,
                Institution = cUser.Institution,
                City = cUser.City,
                Idfaculty = cUser.Idfaculty,
                Password = encrypt.GetSHA256(cUser.Password),
                Idrol = 1
            };

            

            if (ModelState.IsValid & !_context.Users.Any(u => u.Email == cUser.Email))
            {
                _context.Add(user);
                await _context.SaveChangesAsync();

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

                ViewData["Message"] = "Usuario registrado correctamente.";
                return RedirectToAction("Index", "Home");

            }

            ViewData["Idfaculty"] = new SelectList(_context.Faculties, "Idfaculty", "Faculty1", user.Idfaculty);
            ViewData["Message"] = "El usuario ya está registrado.";
            return View(user);

        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Iduser == id);
        }
    }
}
