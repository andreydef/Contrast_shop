using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Pages;

namespace WebStore_Contrast.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // Add GET method the list of pages
        // GET: Admin/Shop/Pages
        [HttpGet]
        public ActionResult Pages(int? page, int? pageId)
        {
            // Assign model PageVM with type List
            List<PageVM> listOfPageVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfPageVM = db.Pages.ToArray()
                    .Where(x => pageId == null || pageId == 0 || x.Id == pageId)
                    .Select(x => new PageVM(x))
                    .ToList();

                // Fill in the pages with data
                ViewBag.Pages = new SelectList(db.Pages.ToList(), "Id", "Title");

                // Set the selected pages
                ViewBag.SelectedPage = pageId.ToString();
            }

            // Set a page navigation
            var onePageOfPages = listOfPageVM.ToPagedList(pageNumber, 5); // 5 - the number of pages in page
            ViewBag.onePageOfPages = onePageOfPages;

            // Return View() with data
            return View(listOfPageVM);
        }

        // Add GET method to Adding pages
        // GET: Admin/Shop/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            // Assign the model of data
            PageVM model = new PageVM();

            // Add the list of pages from database to model
            using (Db db = new Db())
            {
                model.Pages = new SelectList(db.Pages.ToList(), "id", "Title");
            }

            // Return model in view()
            return View(model);
        }

        // Add POST method to Adding pages
        // POST: Admin/Shop/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model, HttpPostedFileBase file)
        {
            // Check model in validation
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Pages = new SelectList(db.Pages.ToList(), "Id", "Title");
                    return View(model);
                }
            }

            // Check the category name for unicity
            using (Db db = new Db())
            {
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    model.Pages = new SelectList(db.Categories.ToList(), "Id", "Title");
                    ModelState.AddModelError("", "The page name is taken!");
                    return View(model);
                }
            }

            // Assign variable PageID
            int id;

            // Initialize and save model on base PageDTO
            using (Db db = new Db())
            {
                PagesDTO page = new PagesDTO();

                page.Title = model.Title;
                page.Slug = model.Slug;
                page.Body = model.Body;

                db.Pages.Add(page);
                db.SaveChanges();

                id = page.Id;
            }

            // Add message in TempData
            TempData["SM"] = "You have added a page!";

            // Redirect user 
            return RedirectToAction("AddPage");
        }

        // Add GET method to Edit Pages
        // GET: Admin/Shop/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Assign model CategoryVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get page
                PagesDTO dto = db.Pages.Find(id);

                // Check, that the page is available 
                if (dto == null)
                {
                    return Content("That page does not exist!");
                }

                // Initialize model to data
                model = new PageVM(dto);

                // Create the list of pages
                model.Pages = new SelectList(db.Pages.ToList(), "Id", "Title");
            }

            // Return model in View()
            return View(model);
        }

        // Add POST method to Edit Pages
        // POST: Admin/Shop/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model, HttpPostedFileBase file)
        {
            // Get ID of page
            int id = model.Id;

            // Fill in the List with pages
            using (Db db = new Db())
            {
                model.Pages = new SelectList(db.Pages.ToList(), "Id", "Name");
            }

            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check the name of page in unicity
            using (Db db = new Db())
            {
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That page name is taken!");
                    return View(model);
                }
            }

            // Update page
            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;
                dto.Slug = model.Slug;
                dto.Body = model.Body;

                db.SaveChanges();
            }

            // Set the message in TempData
            TempData["SM"] = "You have edited the page!";

            // Redirect user
            return RedirectToAction("EditPage");
        }

        // Add GET method to Delete a Page
        // GET: Admin/Shop/DeletePage/id
        [HttpGet]
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                // Get the model of page
                PagesDTO dto = db.Pages.Find(id);

                // Delete page
                db.Pages.Remove(dto);

                // Save changes in database
                db.SaveChanges();
            }

            // Add message about successful delete
            TempData["SM"] = "You have deleted a page!";

            // Return user to the page Pages
            return RedirectToAction("Pages");
        }
    }
}