using Hospital_Appointment_and_Patient_Management_System.Data;
using Hospital_Appointment_and_Patient_Management_System.Models;
using Hospital_Appointment_and_Patient_Management_System.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Appointment_and_Patient_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalAppointments = await _context.Appointments.CountAsync(),
                TodayAppointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .Where(a => a.Date.Date == DateTime.Today)
                    .Select(a => new TodayAppointment
                    {
                        Time = a.Time,
                        PatientName = a.Patient.Name,
                        DoctorName = a.Doctor.Name,
                        Specialization = a.Doctor.Specialization,
                        Status = a.Status
                    })
                    .ToListAsync()
            };

            return View(model);
        }

       
        public async Task<IActionResult> Patients()
        {
            var patients = await _context.Patients.ToListAsync();
            return View(patients);
        }

        
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View(doctors);
        }

       
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();

            return View(appointments);
        }

       

        // GET: Admin/Schedule
        [HttpGet]
        public async Task<IActionResult> Schedule()
        {
            ViewBag.Doctors = await _context.Doctors.ToListAsync();

            var schedules = await _context.DoctorSchedules
                .Include(s => s.Doctor)
                .OrderBy(s => s.Date)
                .ToListAsync();

            return View(schedules);
        }

        // POST: Admin/Schedule (CREATE)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(
            int doctorId,
            DateTime date,
            string timeSlot)
        {
            var schedule = new DoctorSchedule
            {
                DoctorId = doctorId,
                Date = date,
                TimeSlot = timeSlot,
                IsBooked = false
            };

            _context.DoctorSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Schedule created successfully!";
            return RedirectToAction("Schedule");
        }

  

        // GET: Admin/EditSchedule/1
        [HttpGet]
        public async Task<IActionResult> EditSchedule(int id)
        {
            var schedule = await _context.DoctorSchedules
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.DoctorScheduleId == id);

            if (schedule == null)
                return NotFound();

            ViewBag.Doctors = await _context.Doctors.ToListAsync();
            return View(schedule);
        }

        // POST: Admin/EditSchedule
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSchedule(
            int doctorScheduleId,
            int doctorId,
            DateTime date,
            string timeSlot)
        {
            var schedule = await _context.DoctorSchedules
                .FirstOrDefaultAsync(s => s.DoctorScheduleId == doctorScheduleId);

            if (schedule == null)
                return NotFound();

            schedule.DoctorId = doctorId;
            schedule.Date = date;
            schedule.TimeSlot = timeSlot;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Schedule updated successfully!";
            return RedirectToAction("Schedule");
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.DoctorSchedules
                .FirstOrDefaultAsync(s => s.DoctorScheduleId == id);

            if (schedule == null)
                return NotFound();

            _context.DoctorSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Schedule deleted successfully!";
            return RedirectToAction("Schedule");
        }
    }
}
