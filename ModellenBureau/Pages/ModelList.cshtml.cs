﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModellenBureau.Data;
using ModellenBureau.Models;

namespace ModellenBureau
{
    [Authorize(Roles = RoleNames.Admin)]
    public class ModelListModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public List<ModelUser> Users { get; set; }
        //private readonly RoleManager<ModelUser>

        public ModelListModel(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _db.ModelUser.ToListAsync();
            Users.Sort();
            if (Users.Count == 0)
            {
                return NotFound($"Unable to load users.");
            }

            return Page();
        }
    }
}