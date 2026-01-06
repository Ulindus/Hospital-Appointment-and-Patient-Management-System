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

        // ===================== DASHBOARD =====================
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

            if (patient == null)
                return Unauthorized();

            return View(patient);
        }

        // ===================== VIEW DOCTORS =====================
        public async Task<IActionResult> BookAppointment()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View(doctors);
        }

        // ===================== CREATE APPOINTMENT =====================
        [HttpPost]
        public async Task<IActionResult> CreateAppointment(
            int doctorId, DateTime date, string time)
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

            if (patient == null)
                return Unauthorized();

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

            return RedirectToAction("Appointments");
        }

        // ===================== MY APPOINTMENTS =====================
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

        // ===================== CANCEL APPOINTMENT =====================
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
