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
    }
}