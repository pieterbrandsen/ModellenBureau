using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ModellenBureau.Pages
{
    [Authorize(Roles = "Admin")]
    public class testroleModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
