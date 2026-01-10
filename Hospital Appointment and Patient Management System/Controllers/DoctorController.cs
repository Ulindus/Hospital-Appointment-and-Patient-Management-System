using Hospital_Appointment_and_Patient_Management_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Appointment_and_Patient_Management_System.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DoctorController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Doctor Appointments
        public async Task<IActionResult> Appointments()
        {
            var user = await _userManager.GetUserAsync(User);

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.IdentityUserId == user.Id);

            if (doctor == null)
                return Unauthorized();

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctor.Id)
                .ToListAsync();

            return View(appointments);
        }

        
        public IActionResult Schedule()
        {
            return View();
        }

       
        
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Schedule(string[] AvailableDays, string AvailableTime)
        {
            var user = await _userManager.GetUserAsync(User);

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.IdentityUserId == user.Id);

            if (doctor == null)
                return Unauthorized();

            // Save selected days
            if (AvailableDays != null && AvailableDays.Length > 0)
            {
                doctor.AvailableDays = string.Join(", ", AvailableDays);
            }

            // Save time
            doctor.AvailableTime = AvailableTime;

            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Schedule saved successfully!";
            return RedirectToAction("Index");
        }
    }
}
