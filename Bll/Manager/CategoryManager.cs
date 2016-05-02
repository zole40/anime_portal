
using Bll.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll.Manager
{
  public  class CategoryManager
    {

        public static List<CategoryHeaderData> ListCategory()
        {
            using (var context = new AnimePortalEntities())
            {
                var categories = (from p in context.Category
                                  orderby p.Name
                                  select new CategoryHeaderData
                                  {
                                      Id = p.Id,
                                      CategoryName = p.Name    
                                  }).ToList();
                return categories;     
            }
        }


    }
}
