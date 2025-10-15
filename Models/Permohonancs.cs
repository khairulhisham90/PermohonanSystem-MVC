using PermohonanSystemMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PermohonanSystemMVC.Models
{
    public class Permohonan
    {
        public int Id { get; set; }

        [Required]
        public string Tajuk { get; set; }

        public string Penerangan { get; set; }

        public DateTime Tarikh { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        public string? DokumenPath { get; set; }

    }
}
