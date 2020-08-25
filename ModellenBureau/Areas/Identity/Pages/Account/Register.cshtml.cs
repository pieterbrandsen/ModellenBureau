using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ModellenBureau.Data;
using ModellenBureau.Models;

namespace ModellenBureau.Areas.Identity.Pages.Account
{
    [AllowAnonymous]

    public class RegisterModel : PageModel
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;
        public List<SelectListItem> Options { get; set; }

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public ApplicationUser ApplicationUser { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Options = _db.Roles.Where(r => !r.Name.Equals(RoleNames.Admin)).Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.Name.ToString(),
                                              Text = a.Name
                                          }).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                string userRole = Request.Form["userRole"];
                var user = new IdentityUser();
                IdentityResult result = new IdentityResult();
                if (userRole == RoleNames.Customer)
                {
                    user = new CustomerUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.ApplicationUser.FirstName,
                        LastName = Input.ApplicationUser.LastName,
                        Address = Input.ApplicationUser.Address,
                        City = Input.ApplicationUser.City,
                        PostalCode = Input.ApplicationUser.PostalCode,
                        IsVerified = false
                    };
                    result = await _userManager.CreateAsync(user, Input.Password);
                } else if (userRole == RoleNames.Model)
                {
                    user = new ModelUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.ApplicationUser.FirstName,
                        LastName = Input.ApplicationUser.LastName,
                        Address = Input.ApplicationUser.Address,
                        City = Input.ApplicationUser.City,
                        PostalCode = Input.ApplicationUser.PostalCode,
                        IsVerified = false
                    };
                    result = await _userManager.CreateAsync(user, Input.Password);
                }

                // this may become a cleaner solution in the future

                //if (userRole == RoleNames.Customer)
                //{
                //    CustomerUser customerUser = new CustomerUser();
                //    foreach (PropertyInfo prop in user.GetType().GetProperties())
                //    {
                //        typeof(CustomerUser)
                //    }
                //    var result = await _userManager.CreateAsync(user, Input.Password);
                //}
                //else if (userRole == RoleNames.Model)
                //{
                //    //ModelUser modelUser = (ModelUser)user;
                //    var result = await _userManager.CreateAsync((ModelUser)user, Input.Password);
                //}

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _userManager.AddToRoleAsync(user, userRole);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    
                }
                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}

                
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
