using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Shop;

namespace WebStore_Contrast.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            // Declare model with type List
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                // Initialize model to data
                categoryVMList = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }

            // Return List in View()
            return View(categoryVMList);
        }
    }
}