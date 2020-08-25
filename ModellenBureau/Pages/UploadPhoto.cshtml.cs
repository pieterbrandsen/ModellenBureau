using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ModellenBureau.Pages
{
    public class UploadPhotoModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        public UploadPhotoModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }  
        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task OnPostAsync()
        {
            var file = Path.Combine(_environment.ContentRootPath, "uploads", Upload.FileName);
           // _environment.ContentRootPath + "/uploads/picture.jpg"
            using (var fileStream = new FileStream(_environment.ContentRootPath + "/uploads/picture.jpg", FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }
        }
    }
}