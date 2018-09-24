using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;

namespace Tangy.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }


        //Get:Categories/Create

        public IActionResult Create()
        {
            return View();
        }


        //Post: Category

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }





        //Get:Categories/Detail

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var catagory = await _db.Category.SingleOrDefaultAsync(m => m.Id == id);

            if (catagory == null)
            {
                return NotFound();
            }


            return View();
        }



    }
}