using Microsoft.EntityFrameworkCore;
using PermohonanSystemMVC.Models;
using System.Collections.Generic;


namespace PermohonanSystemMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Permohonan> Permohonans { get; set; }
    }
}
