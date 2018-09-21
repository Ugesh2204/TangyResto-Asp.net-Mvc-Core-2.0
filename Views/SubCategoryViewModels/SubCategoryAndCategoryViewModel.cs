using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tangy.Models;

namespace Tangy.Views.SubCategoryViewModels
{
    //Drowp down list category
    public class SubCategoryAndCategoryViewModel
    {
        public SubCategory SubCategory { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }

        public List<string> SubcategoryList { get; set; }

        [Display(Name = "New sub Category")]
        public bool isNew { get; set; }

        public string StatusMessage { get; set; }
    }
}
