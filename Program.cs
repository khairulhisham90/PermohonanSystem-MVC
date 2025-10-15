using PermohonanSystemMVC.Data;
using Microsoft.EntityFrameworkCore;
using PermohonanSystemMVC.Services;   // top
var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Tambah service untuk MVC
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// 2️⃣ Daftar sambungan database (Entity Framework)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IEmailService, EmailService>();  // after AddDbContext / AddControllersWithViews
var app = builder.Build();

// 3️⃣ Middleware: handle error & HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

// 4️⃣ Route default: mula dengan Login page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
