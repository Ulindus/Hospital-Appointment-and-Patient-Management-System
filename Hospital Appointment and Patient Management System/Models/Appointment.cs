using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Appointment_and_Patient_Management_System.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        public DateTime Date { get; set; }

        public string Time { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
    }
}
