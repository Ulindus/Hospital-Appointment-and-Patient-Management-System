using Hospital_Appointment_and_Patient_Management_System.Data;
using Hospital_Appointment_and_Patient_Management_System.Models;
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

        public DoctorController(
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

        
        public IActionResult Schedule()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Schedule(
            string[] AvailableDays,
            string AvailableTime)
        {
            var user = await _userManager.GetUserAsync(User);

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.IdentityUserId == user.Id);

            if (doctor == null)
                return Unauthorized();

          
            doctor.AvailableDays = string.Join(", ", AvailableDays);
            doctor.AvailableTime = AvailableTime;

            
            foreach (var day in AvailableDays)
            {
                var date = GetNextDateForDay(day);

                var schedule = new DoctorSchedule
                {
                    DoctorId = doctor.Id,
                    Date = date,
                    TimeSlot = AvailableTime,
                    IsBooked = false
                };

                _context.DoctorSchedules.Add(schedule);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Schedule created successfully!";
            return RedirectToAction("Schedule");
        }

        
        private DateTime GetNextDateForDay(string day)
        {
            var targetDay = Enum.Parse<DayOfWeek>(day);
            var today = DateTime.Today;

            int daysUntil = ((int)targetDay - (int)today.DayOfWeek + 7) % 7;
            if (daysUntil == 0)
                daysUntil = 7;

            return today.AddDays(daysUntil);
        }
    }
}
