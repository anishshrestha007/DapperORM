using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango.Base.List;
using Bango.Responses;

namespace Test.EventType
{
    public class EventTypeService : Bango.Services.CrudService<EventTypeModel, int?>
    {
        public EventTypeService() {
            DisplayMasterDataFromSystem = true;
        }
        public override ResponseCollection GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            return base.GetComboItems(data_param, page, pageSize, sort_by);
        }
    }
}