using Microsoft.AspNetCore.Mvc;
using PermohonanSystemMVC.Models;
using PermohonanSystemMVC.Data;

namespace PermohonanSystemMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // PAPAR FORM REGISTER
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // HANTAR DATA REGISTER
        [HttpPost]
        public IActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Pastikan emel belum digunakan
                var existingUser = _context.Users.FirstOrDefault(u => u.Emel == model.Emel);
                if (existingUser != null)
                {
                    ViewBag.Message = "Emel ini telah digunakan.";
                    return View(model);
                }

                // Hash password simple (boleh tukar ke BCrypt nanti)
                model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                _context.Users.Add(model);
                _context.SaveChanges();

                ViewBag.Success = "Pendaftaran berjaya. Sila log masuk.";
                return RedirectToAction("Login");
            }


            return View(model);
        }

        // PAPAR FORM LOGIN
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // HANTAR LOGIN
        [HttpPost]
        public IActionResult Login(string emel, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Emel == emel);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                // Simpan dalam session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.Nama);
                HttpContext.Session.SetString("UserRole", user.Role); // 🆕 simpan role

                return RedirectToAction("Index", "Permohonan");
            }

            ViewBag.Message = "Emel atau kata laluan salah.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
