using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tangy.Models.MenuItemViewModels
{
    //Step 2 MI
    public class MenuItemViewModel
    {
        //Object of MenuItem
        public MenuItem MenuItem { get; set; }

        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<SubCategory> SubCategory { get; set; }



        //This view model will be using when creating and editing items
        //Because we need to display all categories asswell as subcategory
        //available in the drop down when a user will be adding a menu item or editing them
        
        //
    }
}
