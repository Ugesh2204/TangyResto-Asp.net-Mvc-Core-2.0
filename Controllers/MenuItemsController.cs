﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Tangy.Data;
using Tangy.Models;
using Tangy.Models.MenuItemViewModels;

namespace Tangy.Controllers
{
    //Step 3 MI
    
    public class MenuItemsController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM{ get; set; }


        public MenuItemsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category.ToList(),
                MenuItem = new Models.MenuItem()
            };
        }
       
        //Get: ManuItems
        public IActionResult Index()
        {
            return View();
        }
    }
}