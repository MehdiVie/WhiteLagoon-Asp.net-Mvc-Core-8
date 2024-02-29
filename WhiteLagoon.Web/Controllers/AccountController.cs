using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext unitOfWork, 
                                UserManager<ApplicationUser> userManager, 
                                SignInManager<ApplicationUser> signInManager, 
                                RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Index), "Home");
            
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Login(string returnUrl=null)
        {
            returnUrl ??= Url.Content("~/");

            LoginVM loginVM = new()
            {
                ReturnUrl = returnUrl
            };

            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe,
                                                                      lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user=await _userManager.FindByEmailAsync(loginVM.Email);
                    if(await _userManager.IsInRoleAsync(user,SD.Role_Admin))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    
                    if (!string.IsNullOrEmpty(loginVM.ReturnUrl))
                    {
                        return LocalRedirect(loginVM.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt!");
                }
            }

            return View(loginVM);
        }

        public IActionResult Register(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult()) 
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();
            }

            RegisterVM registerVM = new()
            {

                RoleList = _roleManager.Roles.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Name
                }),

                RedirectUrl = returnUrl

            };

            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            { 
            
            ApplicationUser user = new()
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                UserName = registerVM.Email,
                NormalizedEmail = registerVM.Email.ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = registerVM.PhoneNumber,
                CreatedAt = DateTime.Now
            };
            

            var result = await  _userManager.CreateAsync(user, registerVM.Password);

            if (result.Succeeded)
            {
                if(!string.IsNullOrEmpty(registerVM.Role))
                {
                    await _userManager.AddToRoleAsync(user, registerVM.Role);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                if(!string.IsNullOrEmpty(registerVM.RedirectUrl))
                {
                    return LocalRedirect(registerVM.RedirectUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }

            else
            {
                foreach (var error in result.Errors)
                {
                  ModelState.AddModelError("", error.Description);
                }
            }

            }


            registerVM.RoleList = _roleManager.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Name
            });

 
            return View(registerVM);
        }
    }
}
