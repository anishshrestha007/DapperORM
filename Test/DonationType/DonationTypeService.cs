using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango.Base.List;
using Bango.Responses;

namespace Test.DonationType
{
    public class DonationTypeService : Bango.Services.CrudService<DonationTypeModel, int?>
    {
        public DonationTypeService() {
            DisplayMasterDataFromSystem = true;
        }
        public override ResponseCollection GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            return base.GetComboItems(data_param, page, pageSize, sort_by);
        }
    }
}