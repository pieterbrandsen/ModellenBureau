using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using ModellenBureau.Data;
using ModellenBureau.Models;

namespace ModellenBureau.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        //public ApplicationUser applicationUser { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public ApplicationUser ApplicationUser { get; set; }
            public ModelUser ModelUser { get; set; }
            public CustomerUser CustomerUser { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var role = await _userManager.GetRolesAsync(user);

            ModelUser modelUser = null;
            CustomerUser customerUser = null;

            switch (role[0])
            {
                case RoleNames.Customer:
                    customerUser = await _db.CustomerUser.FirstOrDefaultAsync(u => u.Id == user.Id);
                    break;

                case RoleNames.Model:
                    modelUser = await _db.ModelUser.FirstOrDefaultAsync(u => u.Id == user.Id);
                    break;

                default:
                    break;
            }
            ApplicationUser applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == user.Id);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                ApplicationUser = applicationUser,
                CustomerUser = customerUser,
                ModelUser = modelUser
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userRole = await _userManager.GetRolesAsync(user);
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (userRole[0] == RoleNames.Customer)
            {
                CustomerUser customerUser = (CustomerUser)await _userManager.GetUserAsync(User);
                customerUser.Website = Input.CustomerUser.Website;
                customerUser.CompanyAddres = Input.CustomerUser.CompanyAddres;
                customerUser.CompanyName = Input.CustomerUser.CompanyName;
                customerUser.KVK_Number = Input.CustomerUser.KVK_Number;
                customerUser.BTW_Number = Input.CustomerUser.BTW_Number;
                customerUser.IsVerified = false;

                var result = await _userManager.UpdateAsync(customerUser);

            }
            else if (userRole[0] == RoleNames.Model)
            {
                ModelUser modelUser = (ModelUser)await _userManager.GetUserAsync(User);
                modelUser.Age = Input.ModelUser.Age;
                modelUser.Height = Input.ModelUser.Height;
                modelUser.Chest = Input.ModelUser.Chest;
                modelUser.LegLength = Input.ModelUser.LegLength;
                modelUser.IsVerified = false;

                var result = await _userManager.UpdateAsync(modelUser);

            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
