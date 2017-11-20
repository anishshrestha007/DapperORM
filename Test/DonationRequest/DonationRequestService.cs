using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango.Base.List;
using Bango.Repo;
using Bango.Responses;

namespace Test.DonationRequest
{
    public class DonationRequestService : Bango.Services.CrudService<donation_request_links, int?>
    {
        public DonationRequestService() {
            DisplayMasterDataFromSystem = true;
        }
        public override ICrudRepo<donation_request_links, int?> InitCrudRepo()
        {
            return new DonationRequestCrudRepo();
        }
        public override ISearchRepo<donation_request_links, int?> InitSearchRepo()
        {
            return new DonationRequestSearchRepo();
        }
        public override ResponseCollection GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            return base.GetComboItems(data_param, page, pageSize, sort_by);
        }
    }
}