using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;

namespace Tangy.Controllers
{
    public class CouponsController : Controller
    {

        private readonly ApplicationDbContext _db;

        public CouponsController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            var result =  await _db.Coupons.ToListAsync();


            return View(result);
        }

        //Create Coupons
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }


        //Create Coupons
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupons coupons)
        {

            if (ModelState.IsValid)
            {
                //if is valid we will retrive the files
                var files = HttpContext.Request.Form.Files;

                //check files is not zero - Null
                if(files[0]!=null && files[0].Length > 0)
                {

                    //convert it into byte
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    coupons.Picture = p1;
                    _db.Coupons.Add(coupons);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
            }

            return View(coupons);
        }


        //Get Edit Coupons
        
        public async Task<IActionResult>Edit (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupons.SingleOrDefaultAsync(m => m.Id == id);

            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }


        //Edit POST mETHOD


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(int id, Coupons coupons)
        {
            if (id != coupons.Id)
            {
                return NotFound();

            }

            var couponsFromDb = await _db.Coupons.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                //check files is not zero - Null
                if (files[0] != null && files[0].Length > 0)
                {

                    //convert it into byte
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    couponsFromDb.Picture = p1;
                }


                couponsFromDb.MinimumAmount = coupons.MinimumAmount;
                couponsFromDb.Name = coupons.Name;
                couponsFromDb.Discount = coupons.Discount;
                couponsFromDb.CouponType = coupons.CouponType;
                couponsFromDb.IsActive = coupons.IsActive;


                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }//

            return View(coupons);
        }




        //Get Delete Coupons

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupons.SingleOrDefaultAsync(m => m.Id == id);

            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupons = await _db.Coupons.SingleOrDefaultAsync(x=> x.Id == id);

            _db.Coupons.Remove(coupons);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}