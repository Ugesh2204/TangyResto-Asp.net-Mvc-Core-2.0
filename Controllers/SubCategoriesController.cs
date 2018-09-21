using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;
using Tangy.Views.SubCategoryViewModels;

namespace Tangy.Controllers
{
    public class SubCategoriesController : Controller
    {
        //Dependency Injection

        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Get Action Asychronous method
        public async Task<IActionResult> Index()
        {
            //Eager Loading

            var subCategories = _db.SubCategory.Include(s => s.Category);

            return View(await subCategories.ToListAsync());
        }


        public IActionResult Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = new SubCategory(),
                SubcategoryList = _db.SubCategory.OrderBy(p => p.Name)
                                    .Select(p => p.Name)
                                    .ToList()
            };


            return View(model);
        }



        //POST Create
        [HttpPost]
        public async Task<IActionResult>Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Where(s => s.Name == model.SubCategory.Name).Count();
                var doesSubCatAndCatExists = _db.SubCategory
                                            .Where(s => s.Name == model.SubCategory.Name 
                                            && s.CategoryId == model.SubCategory.CategoryId).Count();


                if(doesSubCategoryExists > 0 && model.isNew)
                {
                    //error
                    StatusMessage = "Error : Sub Category Name already Exists";
                }
                else
                {
                    if(doesSubCategoryExists == 0 && !model.isNew)
                    {
                        //error
                        StatusMessage = "Error : Sub Category does not exists";
                    }
                    else
                    {
                        if (doesSubCatAndCatExists > 0)
                        {
                            //error
                            StatusMessage = "Error : Category and sub category combination exists";
                        }
                        else
                        {
                            _db.Add(model.SubCategory);
                            await _db.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        

                    }
                }


            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = model.SubCategory,
                SubcategoryList = _db.SubCategory.OrderBy(p => p.Name)
                                    .Select(p => p.Name)
                                    .ToList(),
                StatusMessage = StatusMessage
            
            };

            return View(modelVM);
        }
    }
}