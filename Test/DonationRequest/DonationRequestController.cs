using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango.Base.List;
using Bango.Responses;

namespace Test.DonationRequest
{
    public class DonationRequestController : Bango.Controllers.CrudController<donation_request_links, DonationRequestService, int?>
    {
        public override ResponseModel Post(DynamicDictionary item)
        {
            return base.Post(item);
        }
    }
}