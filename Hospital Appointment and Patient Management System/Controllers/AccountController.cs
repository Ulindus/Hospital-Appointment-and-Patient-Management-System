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

        // ===================== LOGIN =====================

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
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

        // ===================== LOGOUT =====================

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ===================== REGISTER DOCTOR =====================
        // (Recommended: Admin uses this)

        // GET: /Account/RegisterDoctor
        public IActionResult RegisterDoctor()
        {
            return View();
        }
        // GET: /Account/RegisterPatient
        public IActionResult RegisterPatient()
        {
            return View();
        }

        // POST: /Account/RegisterPatient
        [HttpPost]
        public async Task<IActionResult> RegisterPatient(
            string email,
            string password,
            string name)
        {
            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Registration failed";
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Patient");

            var patient = new Patient
            {
                Name = name,
                IdentityUserId = user.Id
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // POST: /Account/RegisterDoctor
        [HttpPost]
        public async Task<IActionResult> RegisterDoctor(
            string email,
            string password,
            string name,
            string specialization,
            string availableTime)
        {
            // 1️⃣ Create Identity user
            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Doctor user creation failed";
                return View();
            }

            // 2️⃣ Assign Doctor role
            await _userManager.AddToRoleAsync(user, "Doctor");

            // 3️⃣ Create Doctor domain record
            var doctor = new Doctor
            {
                Name = name,
                Specialization = specialization,
                AvailableTime = availableTime,
                IdentityUserId = user.Id   // 🔑 CRITICAL LINK
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    }
}
