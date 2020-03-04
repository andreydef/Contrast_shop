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

        // Add GET method the list of categories
        // GET: Admin/Shop/Categories
        [HttpGet]
        public ActionResult Categories(int? page)
        {
            // Assign model BrandVM with type List
            List<CategoryVM> listOfCatVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfCatVM = db.Categories.ToArray()
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }

            // Set a page navigation
            var onePageOfCategories = listOfCatVM.ToPagedList(pageNumber, 5); // 5 - the number of goods in page
            ViewBag.onePageOfCategories = onePageOfCategories;

            // Return View() with data
            return View(listOfCatVM);
        }

        // Add GET method to Adding categories
        // GET: Admin/Shop/AddCategory
        [HttpGet]
        public ActionResult AddCategory()
        {
            // Assign the model of data
            CategoryVM model = new CategoryVM();

            // Add the list of categories from database to model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            // Return model in view()
            return View(model);
        }

        // Add POST method to Adding categories
        // POST: Admin/Shop/AddCategory
        [HttpPost]
        public ActionResult AddCategory(CategoryVM model)
        {
            // Check the category name for unicity
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The category name is taken!");
                    return View(model);
                }
            }

            // Assign variable CategoryID
            int id;

            // Initialize and save model on base CategoryDTO
            using (Db db = new Db())
            {
                CategoryDTO category = new CategoryDTO();

                category.Name = model.Name;
                category.Short_desc = model.Short_desc;
                category.Title = model.Title;
                category.Meta_title = model.Meta_title;
                category.Meta_keywords = model.Meta_keywords;
                category.Meta_description = model.Meta_description;
                category.Body = model.Body;

                db.Categories.Add(category);
                db.SaveChanges();

                id = category.Id;
            }

            // Add message in TempData
            TempData["SM"] = "You have added a category!";

            // Redirect user 
            return RedirectToAction("AddCategory");
        }

        // Add GET method to Edit Categories
        // GET: Admin/Shop/EditCategory/id
        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            // Assign model CategoryVM
            CategoryVM model;

            using (Db db = new Db())
            {
                // Get category
                CategoryDTO dto = db.Categories.Find(id);

                // Check, that the category is available 
                if (dto == null)
                {
                    return Content("That category does not exist!");
                }

                // Initialize model to data
                model = new CategoryVM(dto);
            }

            // Return model in View()
            return View(model);
        }

        // Add POST method to Edit Categories
        // POST: Admin/Shop/EditCategory
        [HttpPost]
        public ActionResult EditCategory(CategoryVM model)
        {
            // Get ID of product 
            int id = model.Id;

            // Fill in the List with categories
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check the name of category in unicity
            using (Db db = new Db())
            {
                if (db.Categories.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That category name is taken!");
                    return View(model);
                }
            }

            // Update category 
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);

                dto.Name = model.Name;
                dto.Short_desc = model.Short_desc;
                dto.Title = model.Title;
                dto.Meta_title = model.Meta_title;
                dto.Meta_keywords = model.Meta_keywords;
                dto.Meta_description = model.Meta_description;
                dto.Body = model.Body;

                db.SaveChanges();
            }

            // Set the message in TempData
            TempData["SM"] = "You have edited the category!";

            // Redirect user
            return RedirectToAction("EditCategory");
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

        #endregion

        #region Brands

        // Add GET method the list of brands
        // GET: Admin/Shop/Brands
        [HttpGet]
        public ActionResult Brands(int? page)
        {
            // Assign model BrandVM with type List
            List<BrandsVM> listOfBrandVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfBrandVM = db.Brands.ToArray()
                    .Select(x => new BrandsVM(x))
                    .ToList();
            }

            // Set a page navigation
            var onePageOfBrands = listOfBrandVM.ToPagedList(pageNumber, 5); // 5 - the number of goods in page
            ViewBag.onePageOfBrands = onePageOfBrands;

            // Return View() with data
            return View(listOfBrandVM);
        }

        // Add GET method to Adding brands
        // GET: Admin/Shop/AddBrand
        [HttpGet]
        public ActionResult AddBrand()
        {
            // Assign the model of data
            BrandsVM model = new BrandsVM();

            // Add the list of categories from database to model
            using (Db db = new Db())
            {
                model.Brands = new SelectList(db.Brands.ToList(), "id", "Name");
            }

            // Return model in view()
            return View(model);
        }

        // Add POST method to Adding brands
        // POST: Admin/Shop/AddBrand
        [HttpPost]
        public ActionResult AddBrand(BrandsVM model)
        {
            // Check the brand name for unicity
            using (Db db = new Db())
            {
                if (db.Brands.Any(x => x.Name == model.Name))
                {
                    model.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The brand name is taken!");
                    return View(model);
                }
            }

            // Assign variable BrandID
            int id;

            // Initialize and save model on base BrandDTO
            using (Db db = new Db())
            {
                BrandsDTO brand = new BrandsDTO();

                brand.Name = model.Name;
                brand.Short_desc = model.Short_desc;
                brand.Meta_title = model.Meta_title;
                brand.Meta_keywords = model.Meta_keywords;
                brand.Meta_description = model.Meta_description;
                brand.Body = model.Body;

                db.Brands.Add(brand);
                db.SaveChanges();

                id = brand.Id;
            }

            // Add message in TempData
            TempData["SM"] = "You have added a brand!";

            // Redirect user 
            return RedirectToAction("AddBrand");
        }

        // Add GET method to Edit Brands
        // GET: Admin/Shop/EditBrand/id
        [HttpGet]
        public ActionResult EditBrand(int id)
        {
            // Assign model ProductVM
            BrandsVM model;

            using (Db db = new Db())
            {
                // Get product (goods)
                BrandsDTO dto = db.Brands.Find(id);

                // Check, that the product is available 
                if (dto == null)
                {
                    return Content("That brand does not exist!");
                }

                // Initialize model to data
                model = new BrandsVM(dto);
            }

            // Return model in View()
            return View(model);
        }

        // Add POST method to Edit Brands
        // POST: Admin/Shop/EditBrand
        [HttpPost]
        public ActionResult EditBrand(BrandsVM model)
        {
            // Get ID of product 
            int id = model.Id;

            // Fill in the List with categories and images
            using (Db db = new Db())
            {
                model.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");
            }

            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check the name of product in unicity
            using (Db db = new Db())
            {
                if (db.Brands.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That brand name is taken!");
                    return View(model);
                }
            }

            // Update product 
            using (Db db = new Db())
            {
                BrandsDTO dto = db.Brands.Find(id);

                dto.Name = model.Name;
                dto.Short_desc = model.Short_desc;
                dto.Meta_title = model.Meta_title;
                dto.Meta_keywords = model.Meta_keywords;
                dto.Meta_description = model.Meta_description;
                dto.Body = model.Body;

                db.SaveChanges();
            }

            // Set the message in TempData
            TempData["SM"] = "You have edited the brand!";

            // Redirect user
            return RedirectToAction("EditBrand");
        }

        // Add GET method to Delete Brand
        // GET: Admin/Shop/DeleteBrand/id
        [HttpGet]
        public ActionResult DeleteBrand(int id)
        {
            using (Db db = new Db())
            {
                // Get the model of category
                BrandsDTO dto = db.Brands.Find(id);

                // Delete category
                db.Brands.Remove(dto);

                // Save changes in database
                db.SaveChanges();
            }

            // Add message about successful delete
            TempData["SM"] = "You have deleted a brand!";

            // Return user to the page Categories
            return RedirectToAction("Brands");
        }

        #endregion
    }
}