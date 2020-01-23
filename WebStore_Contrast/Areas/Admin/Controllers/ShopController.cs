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

        // POST : Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            // Declare string variable ID
            string id;

            using (Db db = new Db())
            {
                // Check the name of categories at unicity
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                // Initialize the model DTO
                CategoryDTO dto = new CategoryDTO();

                // Add data in model
                dto.Name = catName;
                dto.Slug = catName;
                dto.Sorting = 100;

                // Save changes 
                db.Categories.Add(dto);
                db.SaveChanges();

                // Take ID for return in View()
                id = dto.Id.ToString();
            }

            // Return ID in View();
            return id;
        }

        // Add POST method to Sorting Pages
        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                // Realize variable with type Account
                int count = 1;

                // Initialize model of data
                CategoryDTO dto;

                // Set the sorting for the each pages
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        // Add GET method to Delete Category
        // GET: Admin/Shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                // Get the model of category
                CategoryDTO dto = db.Categories.Find(id);

                // Delete category
                db.Categories.Remove(dto);

                // Save changes in database
                db.SaveChanges();
            }

            // Add message about successful delete
            TempData["SM"] = "You have deleted a category!";

            // Return user to the page Categories
            return RedirectToAction("Categories");
        }

        // Add POST method to Rename Category
        // POST: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                // Check the name at unicity
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                // Get model DTO
                CategoryDTO dto = db.Categories.Find(id);

                // Editing model DTO
                dto.Name = newCatName;
                dto.Slug = newCatName;

                // Save changes
                db.SaveChanges();
            }

            // Return string
            return "ok";
        }
    }
}