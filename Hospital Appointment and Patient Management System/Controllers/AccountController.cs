using Hospital_Appointment_and_Patient_Management_System.Data;
using Hospital_Appointment_and_Patient_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Appointment_and_Patient_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(
                email, password, false, false);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "Admin");

            if (roles.Contains("Doctor"))
                return RedirectToAction("Index", "Doctor");

            if (roles.Contains("Patient"))
                return RedirectToAction("Index", "Patient");

            return RedirectToAction("Index", "Home");
        }

        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        
        public IActionResult RegisterDoctor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDoctor(
            string email,
            string password,
            string name,
            string specialization,
            string availableTime)
        {
            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join(", ",
                    result.Errors.Select(e => e.Description));
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Doctor");

            var doctor = new Doctor
            {
                Name = name,
                Specialization = specialization,
                AvailableTime = availableTime,
                IdentityUserId = user.Id
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        public IActionResult RegisterPatient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPatient(
            string email,
            string password,
            string name,
            string phoneNumber, string gender)   
        {
            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join(", ",
                    result.Errors.Select(e => e.Description));
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Patient");

            var patient = new Patient
            {
                Name = name,
                PhoneNumber = phoneNumber,
                Gender = gender,
                IdentityUserId = user.Id
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    }
}
