using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModellenBureau.Data;
using ModellenBureau.Models;

namespace ModellenBureau.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public List<InputModel> FirstPhotos = new List<InputModel>();
        public IndexModel(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public class InputModel{
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public string City { get; set; }
            public FileModel PhotoPath { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            List<ModelUser> users = await _db.ModelUser.Include(m => m.Photos).ToListAsync();
            users.Sort();

            var firstPhotos = new List<FileModel>();

            foreach (var user in users)
            {
                if (user.Photos.Count > 0)
                    FirstPhotos.Add(new InputModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Age = user.Age,
                        City = user.City,
                        PhotoPath = user.Photos.FirstOrDefault()
                    }) ;

            }

            //if (Users.Count == 0)
            //{
            //    return NotFound($"Unable to load users.");
            //}

            return Page();
        }
    }
}
