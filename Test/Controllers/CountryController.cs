using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango.Responses;
using Test.Product;

namespace Test.Controllers
{
    public class CountryController:Bango.Controllers.CrudController<CountryModel,CountryService,int?>
    {
        public override ResponseCollection Get(int page = 1, int page_size = 20, string sort_by = null)
        {
            return base.Get(page, page_size, sort_by);
        }
    }
}