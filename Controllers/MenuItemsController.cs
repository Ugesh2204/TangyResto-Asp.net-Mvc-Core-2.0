using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;
using Tangy.Models.MenuItemViewModels;
using Tangy.Utility;

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
       
        //Get: MenuItems
        public async Task<IActionResult> Index()
        {
            //Eager loading 
            var menuItems = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory);
            return View(await menuItems.ToListAsync());
        }


        //Get :MenuItems create

        public IActionResult Create()
        {
            return View(MenuItemVM);
        }


        //POST:MenuItem Create
        [HttpPost,ActionName("Create")]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubcategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();


            //save image
            //this will retive the www root folder
            string WebRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;


            var menuItemFromDb = _db.MenuItem.Find(MenuItemVM.MenuItem.Id);


            if(files[0]!=null && files[0].Length > 0)
            {
                //when user uploads an image
                var uploads = Path.Combine(WebRootPath, "images");
                var extension = files[0]
                                .FileName
                                .Substring(files[0].FileName.LastIndexOf("."),
                                files[0].FileName.Length-files[0].FileName.LastIndexOf("."));

                using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }

            else
            {
                //when user does not upload and image 
                var uploads = Path.Combine(WebRootPath, @"images\"+ SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, WebRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image= @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }




        //Loading subcatogry from database 
        public JsonResult GetSubCategory(int CategoryId)
        {
            List<SubCategory> subCategoryList = new List<SubCategory>();

            subCategoryList = (from subCategory in _db.SubCategory
                               where subCategory.CategoryId == CategoryId
                               select subCategory).ToList();
            return Json(new SelectList(subCategoryList, "Id", "Name"));
        }


        //Get : Menu item
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category)
                                  .Include(m => m.SubCategory)
                                  .SingleOrDefaultAsync(m => m.Id == id);

            MenuItemVM.SubCategory = _db.SubCategory
                                     .Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId)
                                     .ToList();

            if(MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }


            return View(MenuItemVM);
        }


        //POST : Edit MenuItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {

            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubcategoryId"].ToString());

            if (id != MenuItemVM.MenuItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var menuItemFromDb = _db.MenuItem.Where(m => m.Id == MenuItemVM.MenuItem.Id)
                                         .FirstOrDefault();

                    if(files[0].Length>0 && files[0] != null)
                    {
                        //if user uploads a new image
                        var uploads = Path.Combine(webRootPath, "images");

                        var extension_New = files[0]
                              .FileName
                              .Substring(files[0].FileName.LastIndexOf("."),
                              files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                        var extension_Old = 
                               menuItemFromDb.Image
                               .Substring(menuItemFromDb.Image.LastIndexOf("."),
                               menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf("."));


                        if (System.IO.File.Exists(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_Old)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_Old));
                        }

                        using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_New), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }

                        MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_New;
                    }


                    if(MenuItemVM.MenuItem.Image != null)
                    {
                        menuItemFromDb.Image = MenuItemVM.MenuItem.Image;
                    }

                    menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
                    menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
                    menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
                    menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
                    menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
                    menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;
                    await _db.SaveChangesAsync();



                }

                catch (Exception ex)
                {

                }
                return RedirectToAction(nameof(Index));
               
            }
            MenuItemVM.SubCategory = _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToList();
            return View(MenuItemVM);

        }


        //Get : Detils Menu item
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category)
                                  .Include(m => m.SubCategory)
                                  .SingleOrDefaultAsync(m => m.Id == id);

            

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }


            return View(MenuItemVM);
        }
    }

}