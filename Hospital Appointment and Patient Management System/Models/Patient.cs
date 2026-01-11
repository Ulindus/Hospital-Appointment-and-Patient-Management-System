using System.ComponentModel.DataAnnotations;

namespace Hospital_Appointment_and_Patient_Management_System.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        public string Name { get; set; } = string.Empty;

        

        
        public string Phone { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string IdentityUserId { get; set; } = string.Empty;
    }
}
