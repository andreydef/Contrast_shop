using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Shop;

namespace WebStore_Contrast.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        // GET: Shop/CategoryMenuPartial
        public ActionResult CategoryMenuPartial(CategoryVM model)
        {
            // Assign the model with type List<> CategoryVM
            List<CategoryVM> categoryVMList;

            // Initialize the model to data
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray()
                    .Select(x => new CategoryVM(x)).ToList();

                if (db.Categories.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The category name is taken!");
                    return View(model);
                }
            }

            // Return PartialView() with model 
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // Assign the models DTO and VM
            ProductDTO dto;
            ProductVM model;

            // Assign and initialize ID of product
            int id = 0;

            using (Db db = new Db())
            {
                // Check, that the product is available 
                if (!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // Initialize the model DTO to data
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                // Get ID
                id = dto.Id;

                // Initialize the model VM to data 
                model = new ProductVM(dto);
            }

            // Get the images from gallery
            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Return the model in View()
            return View("ProductDetails", model);
        }
    }
}