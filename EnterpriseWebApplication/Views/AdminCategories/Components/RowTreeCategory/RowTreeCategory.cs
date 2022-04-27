using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseWebApplication.Views.AdminCategories.Components.RowTreeCategory
{
    [ViewComponent]
    public class RowTreeCategory : ViewComponent
    {
        public RowTreeCategory()
        {

        }
        // data là sữ liệu có cấu trúc
        // { 
        //    categories - danh sách các Category
        //    level - cấp của các Category 
        // }
        public IViewComponentResult Invoke(dynamic data)
        {
            return View(data);
        }
    }
}
