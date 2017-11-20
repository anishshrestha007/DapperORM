using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using Bango.Repo;
using Bango.Responses;

namespace Test.Event
{
    public class EventService : Bango.Services.CrudService<EventModel, int?>
    {
        public EventService() {
            DisplayMasterDataFromSystem = true;
        }
       
        public override ResponseCollection GetSearchItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            return base.GetSearchItems(data_param, page, pageSize, sort_by);
        }
        public override ResponseModel Get(DbConnect con, int? id)
        {
            return base.Get(con, id);
        }
        public override ResponseCollection GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            return base.GetComboItems(data_param, page, pageSize, sort_by);
        }
    }
}