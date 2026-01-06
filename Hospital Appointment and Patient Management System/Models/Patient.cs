using System.ComponentModel.DataAnnotations;

namespace Hospital_Appointment_and_Patient_Management_System.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        // ✅ ADD THIS (FIXES YOUR ERROR)
        public string Phone { get; set; } = string.Empty;

        // 🔑 Link to ASP.NET Identity
        public string IdentityUserId { get; set; } = string.Empty;
    }
}
