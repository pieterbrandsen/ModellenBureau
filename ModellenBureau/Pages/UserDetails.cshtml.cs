using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModellenBureau.Data;
using ModellenBureau.Models;

namespace ModellenBureau.Pages
{
    public class UserDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CustomerUser customerUser { get; set; }
        public ModelUser modelUser { get; set; }

        public string userRole { get; set; }
        public UserDetailsModel(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task OnGetAsync(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if(await _userManager.IsInRoleAsync(user, RoleNames.Model))
            {
                modelUser = _db.ModelUser.Include(m=>m.Photos).FirstOrDefault(u => u.Id == Id);
                userRole = RoleNames.Model;
                //await _db.FindAsync(modelUser.GetType(), new { Id = Id})
            }
            else if (await _userManager.IsInRoleAsync(user, RoleNames.Customer))
            {
                customerUser = _db.CustomerUser.FirstOrDefault(u => u.Id == Id);
                userRole = RoleNames.Customer;
            }
        }
        public async Task<RedirectToPageResult> OnPostAsync(string id) {
            var user = await _userManager.FindByIdAsync(id);
            ApplicationUser appUser = (ApplicationUser)user;
            appUser.IsVerified = true;
            var succes = await _userManager.UpdateAsync(appUser);
            if (succes == IdentityResult.Success) { }
                
            return RedirectToPage(user);

            //await _userManager.UpdateAsync()
        }
    }
}