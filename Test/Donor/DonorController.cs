using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Donor
{
    public class DonorController:Bango.Controllers.CrudController<DonorModel, DonorService, int?>
    {
    }
}