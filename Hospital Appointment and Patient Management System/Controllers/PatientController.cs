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

       
        public IActionResult Index()
        {
            return View();
        }

        
        public async Task<IActionResult> BookAppointment()
        {
            var schedules = await _context.DoctorSchedules
                .Include(s => s.Doctor)
                .Where(s => !s.IsBooked && s.Date >= DateTime.Today)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.TimeSlot)
                .ToListAsync();

            return View(schedules);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateAppointment(int scheduleId)
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

            if (patient == null)
                return Unauthorized();

            var schedule = await _context.DoctorSchedules
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.DoctorScheduleId == scheduleId);

            if (schedule == null || schedule.IsBooked)
            {
                TempData["Error"] = "Schedule is no longer available.";
                return RedirectToAction("BookAppointment");
            }

            var appointment = new Appointment
            {
                DoctorId = schedule.DoctorId,
                PatientId = patient.PatientId,
                Date = schedule.Date,
                Time = schedule.TimeSlot,
                Status = "Pending"
            };

            schedule.IsBooked = true;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction("Appointments");
        }

        
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
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(appointments);
        }

        
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

            
            var schedule = await _context.DoctorSchedules.FirstOrDefaultAsync(s =>
                s.DoctorId == appointment.DoctorId &&
                s.Date == appointment.Date &&
                s.TimeSlot == appointment.Time);

            if (schedule != null)
                schedule.IsBooked = false;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment cancelled.";
            return RedirectToAction("Appointments");
        }
    }
}
