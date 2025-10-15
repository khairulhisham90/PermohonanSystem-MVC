using System.ComponentModel.DataAnnotations;

namespace PermohonanSystemMVC.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Nama { get; set; }

        [Required, EmailAddress]
        public string Emel { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; } = "USER"; // 🆕 default user biasa
    }
}
