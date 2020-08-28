using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModellenBureau.Data;
using ModellenBureau.Models;

namespace ModellenBureau.Pages
{
    [Authorize(Roles = RoleNames.Model + ", " + RoleNames.Admin)]
    public class UploadPhotoModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        public UploadPhotoModel(IWebHostEnvironment environment, 
                                UserManager<IdentityUser> userManager,
                                ApplicationDbContext db)
        {
            _environment = environment;
            _userManager = userManager;
            _db = db;
        }  
        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task OnPostAsync()
        {

            var user = await _userManager.GetUserAsync(User);
            var directoryPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", user.Id);
            Directory.CreateDirectory(directoryPath);

            var file = Path.Combine(directoryPath, Path.GetFileName(Upload.FileName));

            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                if(Upload.ContentType.ToString().Contains("image"))
                        await Upload.CopyToAsync(fileStream);
            }

            var photo = new FileModel { Id = Guid.NewGuid().ToString(), FilePath = Path.Combine("uploads", user.Id, Path.GetFileName(file)) };

            //check for double upload
            if(_db.FileModel.Where(f => f.FilePath == file).ToArray().Length == 0)
            {

                var modelUser = _db.ModelUser.Include(m => m.Photos).FirstOrDefault(u => u.Id == user.Id);
                modelUser.Photos.Add(photo);

                await _userManager.UpdateAsync(modelUser);
            }

            

        }
    }
}