using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Pages;
using WebStore_Contrast.Models.ViewModels.Shop;

namespace WebStore_Contrast.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            // Get/set the short title (SLUG)
            if (page == "")
            {
                page = "home";
            }

            // Assign the model and class DTO
            PageVM model;
            PagesDTO dto;

            // Check, that the page is available
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            // Get DTO of page
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            // Set the title of page (TITLE)
            ViewBag.PageTitle = dto.Title;

            // Fill the model to data
            model = new PageVM(dto);

            // Return the view() with model
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            // Initialize list PageVM
            List<PageVM> pageVMList;

            // Get all pages, except HOME
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().Where(x => x.Slug != "home")
                    .Select(x => new PageVM(x)).ToList();
            }

            // Return partial view() with list of data
            return PartialView("_PagesMenuPartial", pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            // Assign the model
            SidebarVM model;

            // Initialize the model of data 
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);

                model = new SidebarVM(dto);
            }

            // Return model in partial view()
            return PartialView("_SidebarPartial", model);
        }
    }
}