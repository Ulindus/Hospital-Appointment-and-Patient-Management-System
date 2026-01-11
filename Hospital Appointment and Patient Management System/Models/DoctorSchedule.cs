namespace Hospital_Appointment_and_Patient_Management_System.Models
{
    public class DoctorSchedule
    {
        public int DoctorScheduleId { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public DateTime Date { get; set; }

        public string TimeSlot { get; set; } // e.g. 09:00 - 10:00

        public bool IsBooked { get; set; } = false;
    }
}
