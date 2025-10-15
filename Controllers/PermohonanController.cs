using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermohonanSystemMVC.Data;
using PermohonanSystemMVC.Models;
using PermohonanSystemMVC.Services;

namespace PermohonanSystemMVC.Controllers
{
    public class PermohonanController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _email;

        public PermohonanController(AppDbContext context, IEmailService email)
        {
            _context = context;
            _email = email;
        }

        // ============================================================
        // SENARAI PERMOHONAN
        // ============================================================
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            List<Permohonan> data;

            if (userRole == "ADMIN")
            {
                // admin lihat semua
                data = _context.Permohonans
                    .Include(p => p.User) // join table user
                    .OrderByDescending(p => p.Tarikh)
                    .ToList();
            }
            else
            {
                // user biasa lihat permohonan sendiri
                int uid = int.Parse(userId);
                data = _context.Permohonans
                    .Include(p => p.User)
                    .Where(p => p.UserId == uid)
                    .OrderByDescending(p => p.Tarikh)
                    .ToList();
            }

            return View(data);
        }

        // ============================================================
        // CREATE
        // ============================================================
        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Permohonan model, IFormFile? Dokumen)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Simpan fail kalau ada
                if (Dokumen != null && Dokumen.Length > 0)
                {
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Dokumen.FileName);
                    string fullPath = Path.Combine(uploadPath, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await Dokumen.CopyToAsync(stream);
                    }

                    model.DokumenPath = "/uploads/" + uniqueFileName;
                }

                model.UserId = int.Parse(userId);
                model.Tarikh = DateTime.Now;

                _context.Permohonans.Add(model);
                _context.SaveChanges();

                // --- Hantar emel kepada admin ---
                string? physicalAttachment = null;
                if (!string.IsNullOrEmpty(model.DokumenPath))
                {
                    physicalAttachment = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        model.DokumenPath.TrimStart('/')
                    );
                }

                var subject = $"Permohonan Baru #{model.Id} – {model.Tajuk}";
                var body = $@"
                    <p>Admin,</p>
                    <p>Ada permohonan baru dihantar oleh pengguna ID: <b>{model.UserId}</b></p>
                    <ul>
                        <li><b>Tajuk:</b> {model.Tajuk}</li>
                        <li><b>Penerangan:</b> {model.Penerangan}</li>
                        <li><b>Tarikh:</b> {model.Tarikh:dd/MM/yyyy HH:mm}</li>
                    </ul>
                    {(string.IsNullOrEmpty(model.DokumenPath)
                        ? "<p><i>Tiada dokumen dilampirkan.</i></p>"
                        : $"<p>Lampiran: <a href=\"{model.DokumenPath}\">lihat fail</a></p>")}
                ";

                _ = Task.Run(() => _email.SendAsync(subject, body, physicalAttachment));
                // --- Tamat emel ---

                TempData["Success"] = "Permohonan berjaya dihantar.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var permohonan = _context.Permohonans.FirstOrDefault(p => p.Id == id && p.UserId == int.Parse(userId));
            if (permohonan == null)
                return NotFound();

            return View(permohonan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Permohonan model, IFormFile? Dokumen)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var existing = _context.Permohonans.FirstOrDefault(p => p.Id == model.Id && p.UserId == int.Parse(userId));
            if (existing == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                existing.Tajuk = model.Tajuk;
                existing.Penerangan = model.Penerangan;
                existing.Tarikh = DateTime.Now;

                // jika ada fail baru
                if (Dokumen != null && Dokumen.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existing.DokumenPath))
                    {
                        string oldFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.DokumenPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFile))
                            System.IO.File.Delete(oldFile);
                    }

                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Dokumen.FileName);
                    string fullPath = Path.Combine(uploadPath, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await Dokumen.CopyToAsync(stream);
                    }

                    existing.DokumenPath = "/uploads/" + uniqueFileName;
                }

                _context.SaveChanges();
                TempData["Success"] = "Permohonan berjaya dikemaskini.";
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
