using System.ComponentModel.DataAnnotations;

namespace Hospital_Appointment_and_Patient_Management_System.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

       
        public string? AvailableTime { get; set; } = string.Empty;
        public string? AvailableDays { get; set; }

        // 🔗 Identity link
        public string IdentityUserId { get; set; }
    }
}
