using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Shop;

namespace WebStore_Contrast.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        #region Products
        // Add GET method to Adding goods
        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Assign the model of data
            ProductVM model = new ProductVM();

            // Add the list of categories from database to model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            // Return model in view()
            return View(model);
        }

        // Add POST method to Adding goods
        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            // Check model in validation
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Check the product name for unicity
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The product name is taken!");
                    return View(model);
                }
            }

            // Assign variable ProductID
            int id;

            // Initialize and save model on base ProductDTO
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;
            }

            // Add message in TempData
            TempData["SM"] = "You have added a product!";

            #region Upload Image

            // Create the necessary links of directories
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            // Check availability of directories (if not, create)
            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Check that the file has been downloaded 
            if (file != null && file.ContentLength > 0)
            {
                // Get the file extension
                string ext = file.ContentType.ToLower();

                // Check file extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png" &&
                    ext != "image/xbm" &&
                    ext != "image/tif" &&
                    ext != "image/pjp" &&
                    ext != "image/jfif" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/ico" &&
                    ext != "image/tiff" &&
                    ext != "image/svg" &&
                    ext != "image/bmp" &&
                    ext != "image/svgz" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/webp")   // A few (рідкісне) image extension but sometimes used
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension!");
                        return View(model);
                    }
                }

                // Assign variable with name of image
                string imageName = file.FileName;

                // Save the name of image in model DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Assign paths to the original and reduced image
                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                // Save original image
                file.SaveAs(path);

                // Create and save reduced copy of image
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }
            #endregion

            // Redirect user 
            return RedirectToAction("AddProduct");
        }

        // Add GET method the list of goods
        // GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            // Assign model ProductVM with type List
            List<ProductVM> listOfProductVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();

                // Fill in the categories with data
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Set the selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            // Set a page navigation
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 5); // 5 - the number of goods in page
            ViewBag.onePageOfProducts = onePageOfProducts;

            // Return View() with data
            return View(listOfProductVM);
        }

        // Add GET method to Edit Products
        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            // Assign model ProductVM
            ProductVM model;

            using (Db db = new Db())
            {
                // Get product (goods)
                ProductDTO dto = db.Products.Find(id);

                // Check, that the product is available 
                if (dto == null)
                {
                    return Content("That product does not exist!");
                }

                // Initialize model to data
                model = new ProductVM(dto);

                // Create the list of categories
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Get all images from gallery
                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }

            // Return model in View()
            return View(model);
        }

        // Add POST method to Edit Products
        // POST: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            // Get ID of product 
            int id = model.Id;

            // Fill in the List with categories and images
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check the name of product in unicity
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }

            // Update product 
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name;
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            // Set the message in TempData
            TempData["SM"] = "You have edited the product!";

            // Realize the logic of image processing

            #region Image Upload

            // Check the file downloading
            if (file != null && file.ContentLength > 0)
            {
                // Get the image extension
                string ext = file.ContentType.ToLower();

                // Check the extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png" &&
                    ext != "image/xbm" &&
                    ext != "image/tif" &&
                    ext != "image/pjp" &&
                    ext != "image/jfif" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/ico" &&
                    ext != "image/tiff" &&
                    ext != "image/svg" &&
                    ext != "image/bmp" &&
                    ext != "image/svgz" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/webp")   // A few (рідкісне) image extension but sometimes used
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension!");
                        return View(model);
                    }
                }

                // Set path for download
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // Delete the existent files and directories 
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                // Save the image
                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Save the original and preview version 
                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

                // Save original image
                file.SaveAs(path);

                // Create and save reduced copy of image
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }
            #endregion

            // Redirect user
            return RedirectToAction("EditProduct");
        }

        // Add POST method to Delete Products
        // POST: Admin/Shop/DeleteProduct/id
        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            // Delete product from database 
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);
                db.SaveChanges();
            }

            // Delete the directories of product (images)
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            // Redirect user
            return RedirectToAction("Products");
        }

        // Add POST method to Add images to Gallery
        // POST: Admin/Shop/SaveGalleryImages/id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Sort all getting files
            foreach (string fileName in Request.Files)
            {
                // Initialize the files
                HttpPostedFileBase file = Request.Files[fileName];

                // Check on null
                if (file != null && file.ContentLength > 0)
                {
                    // Assign all paths to directories
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // Assign paths of images
                    var path = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    // Save original and reduced copies of images
                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1, 1);
                    img.Save(path2);
                }
            }
        }

        // Add POST method to Delete images from Gallery
        // POST: Admin/Shop/DeleteImage/id/imageName
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs" + imageName);

            // Check that the images is available 
            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
        }
        #endregion
        #region Categories
        // GET: Admin/Shop
        [HttpGet]
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

        // Add POST method to Sorting Categories
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
        [HttpGet]
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
        #endregion
    }
}