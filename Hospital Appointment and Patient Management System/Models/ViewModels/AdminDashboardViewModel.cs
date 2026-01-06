namespace Hospital_Appointment_and_Patient_Management_System.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }

        public List<TodayAppointment> TodayAppointments { get; set; }
    }

    public class TodayAppointment
    {
        public string Time { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string Status { get; set; }
    }
}
