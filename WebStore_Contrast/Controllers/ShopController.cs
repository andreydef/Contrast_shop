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
        public ActionResult CategoryMenuPartial()
        {
            // Assign the model with type List<> CategoryVM
            List<CategoryVM> categoryVMList;

            // Initialize the model to data
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray()
                    .Select(x => new CategoryVM(x)).ToList();
            }

            // Return PartialView() with model 
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            // Assign the list with type List<>
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                // Get ID of category
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Short_desc == name).FirstOrDefault();

                int catId = categoryDTO.Id;

                // Initialize the list to data
                productVMList = db.Products.ToArray()
                                    .Where(x => x.CategoryId == catId)
                                    .Select(x => new ProductVM(x))
                                    .ToList();

                // Get the name of category 
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                // Doing a check on NULL
                if (productCat == null)
                {
                    var catName = db.Categories
                        .Where(x => x.Short_desc == name)
                        .Select(x => x.Name)
                        .FirstOrDefault();

                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }

            // Return View() with model
            return View(productVMList);
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
                if (!db.Products.Any(x => x.Short_desc.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // Initialize the model DTO to data
                dto = db.Products.Where(x => x.Short_desc == name).FirstOrDefault();

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