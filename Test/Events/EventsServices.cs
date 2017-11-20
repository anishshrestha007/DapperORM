using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using Bango.Responses;

namespace Test.Events
{
    public class EventsServices : Bango.Services.CrudService<EventsModel, int?>
    {
        public EventsServices()
        {
            DisplayMasterDataFromSystem = true;
        }
        public override ResponseModel Insert(DbConnect con, DynamicDictionary item)
        {
            return base.Insert(con, item);
        }
    }
}