using Hospital_Appointment_and_Patient_Management_System.Data;
using Hospital_Appointment_and_Patient_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Appointment_and_Patient_Management_System.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PatientController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

            if (patient == null)
                return Unauthorized();

            return View(patient);
        }

    
        public async Task<IActionResult> BookAppointment()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View(doctors);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAppointment(
    int doctorId, DateTime date, string time)
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

            if (patient == null)
                return Unauthorized();

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
                return NotFound();

            
            if (!string.IsNullOrEmpty(doctor.AvailableDays))
            {
                var selectedDay = date.DayOfWeek.ToString(); // Monday, Tuesday, etc.

                var allowedDays = doctor.AvailableDays
                    .Split(',')
                    .Select(d => d.Trim())
                    .ToList();

                if (!allowedDays.Contains(selectedDay))
                {
                    TempData["Error"] =
                        $"Doctor is not available on {selectedDay}. Available days: {doctor.AvailableDays}";
                    return RedirectToAction("BookAppointment");
                }
            }
            

            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patient.PatientId,
                Date = date,
                Time = time,
                Status = "Pending"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction("Appointments");
        }


        //MY APPOINTMENTS 
        public async Task<IActionResult> Appointments()
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

            if (patient == null)
                return Unauthorized();

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patient.PatientId)
                .ToListAsync();

            return View(appointments);
        }

        //
        // CANCEL APPOINTMENT 
        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a =>
                    a.AppointmentId == id &&
                    a.PatientId == patient.PatientId);

            if (appointment == null)
                return Unauthorized();

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return RedirectToAction("Appointments");
        }
    }
}
