using Blog3.Services.Email;
using Blog3.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.Controllers
{
    public class AuthController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private IEmailService _emailService;

        public AuthController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager , IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel()); ;
        }

        

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            //var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, false, false);
            if (vm.Username == null) vm.Username = " ";
            if (vm.Password == null) vm.Password = " ";
            
            var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, false, false);
            //redirect only admin to admin panel
            if (!result.Succeeded)
            {
                vm.IsValid = false;
                return View(vm);
            }

            //get the user we are dealing with 
            var user = await _userManager.FindByNameAsync(vm.Username);
            
           
            //if the user is admin we take him to admin panel if not we take him to home page
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                return RedirectToAction("Index", "Panel");

            }
            return RedirectToAction("Index", "Home");

            //return View();
        }

        [HttpGet]
        public async  Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
            //return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel()); ;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                //return RedirectToAction("Errorr", "Auth");
                return View(vm);
            }


            var user = new IdentityUser
            {
                //UserName = vm.Email,
                UserName = vm.BlogUsername,
                Email = vm.Email,
                
            };







            var existing_user = await _userManager.FindByNameAsync(vm.BlogUsername);
            if (existing_user != null)
            {
                vm.UsernameInUse = true;
                return View(vm);
            }

            var existing_user2 = await _userManager.FindByEmailAsync(vm.Email);
            if (existing_user2 != null)
            {
                vm.EmailInUse = true;
                return View(vm);
            }








            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user,false);
                    //await _emailService.SendEmail(user.Email, "Welcome", "Thank you for registering");
                    //await _emailService.SendEmail(user.Email, "Welcome", "Thank you for registering");
                return RedirectToAction("Index", "Home");

            }

            return View(vm);
        }
    }
}
