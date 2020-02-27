using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStore_Contrast.Models.Data;

namespace WebStore_Contrast.Models.ViewModels.Shop
{
    public class CategoryVM
    {
        public CategoryVM()
        {

        }

        public CategoryVM(CategoryDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}