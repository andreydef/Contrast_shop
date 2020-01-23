using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Pages;

namespace WebStore_Contrast.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //  Create list for ViewModels (PageVM)
            List<PageVM> pageList;

            // Initialize list (Db)
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            // Return list to PageVM
            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet] // this method a receives and displays data
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost] // this method processes the data
        public ActionResult AddPage(PageVM model)
        {
            // Validation of the model
            if (!ModelState.IsValid) // if model is not valid
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Declare a variable for short description (slug)
                string slug;

                // Initialize class PageDTO
                PagesDTO dto = new PagesDTO();

                // Assign the model title
                dto.Title = model.Title;

                // Check if there is a short description(slug), if not, assign it
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title;
                }
                else
                {
                    slug = model.Slug;
                }

                // Make sure that the title and short description(slug) - are unique
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model); // return model with taken data
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model); // return model with taken data
                }

                // After that, assign the values of the model, which have saved
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100; // When adding a model to a database, the model was added to the end of the database list

                // Saved model to the database
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            // Send notifications through the TempData
            TempData["SM"] = "You have added a new page!";

            // Redirected the user to the method INDEX
            return RedirectToAction("Index");
        }

        // Add GET method to Edit Pages
        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Create models PageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get page data
                PagesDTO dto = db.Pages.Find(id);

                // Checking, if a page is available
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                // Initialize model dates (Ініціалізуємо модель данними)
                model = new PageVM(dto);

                // Return model
                return View(model);
            }
        }

        // Add POST method to Edit Pages
        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            // Check model for validity
            if (!ModelState.IsValid) // if model does not valid
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Get ID of the page
                int id = model.Id;

                // Create variable for short title (temporary variable)
                string slug = "home";

                // Get page (by id)
                PagesDTO dto = db.Pages.Find(id);

                // Assign a title for the resulting model to DTO
                dto.Title = model.Title;

                // Check the short title and assign it, if necessary
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug)) // null or space
                    {
                        slug = model.Title;
                    }
                    else
                    {
                        slug = model.Slug;
                    }
                }

                // Check sluh and title in unique
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))  // Search the database for all ID in addition to the ID that is entered at the moment,
                                                                                       // and if Title matches the database with what we entered then we add an error
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model);
                }

                // Write other values to the class DTO
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // Save Changes in database
                db.SaveChanges();
            }

            // Send notifications through the TempData
            TempData["SM"] = "You have esited the page.";

            // Redirected the user
            return RedirectToAction("EditPage");
        }

        // Add GET method to Page Details
        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            // Assign model PageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get page
                PagesDTO dto = db.Pages.Find(id);

                // Confirm, that page is available 
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                // Assogn to model information from database
                model = new PageVM(dto);
            }

            // Return model in View
            return View(model);
        }

        // Add GET method to Delete Pages
        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                // Get page
                PagesDTO dto = db.Pages.Find(id);

                // Delete page
                db.Pages.Remove(dto);

                // Save changes in database
                db.SaveChanges();
            }

            // Add message about successful delete
            TempData["SM"] = "You have deleted a page!";

            // Return user to the page INDEX
            return RedirectToAction("Index");
        }

        // Add POST method to Sorting Pages
        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                // Realize variable with type Account
                int count = 1;

                // Initialize model of date
                PagesDTO dto;

                // Set the sorting for the each pages
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        // Add GET method to Edit Sidebar
        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // Declare model
            SidebarVM model;

            using (Db db = new Db())
            {
                // Take data on the database
                SidebarDTO dto = db.Sidebars.Find(1); /* test version, I fix this later - don't try to do this!!! (bad code - Hard values ​​cannot be added!) */

                // Fill the model with data
                model = new SidebarVM(dto);
            }

            // Return View with model
            return View(model);
        }

        // Add POST method to Edit Sidebar
        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                // Get gata with DTO
                SidebarDTO dto = db.Sidebars.Find(1); /* test version, I fix this later - don't try to do this!!! (bad code - Hard values ​​cannot be added!) */

                // Assign data to the body (in property Body) 
                dto.Body = model.Body;

                // Save
                db.SaveChanges();
            }

            // Assign message in TempData
            TempData["SM"] = "You have edited the Sidebar!";

            // Redirect user (admin)
            return RedirectToAction("EditSidebar");
        }
    }
}