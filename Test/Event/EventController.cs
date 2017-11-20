using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango.Base.List;
using Bango.Responses;
using Bango.Controllers;
using Bango;
using System.Web.Http.Cors;

namespace Test.Event
{
    public class EventController : Bango.Controllers.CrudController<EventModel, EventService, int?>
    {
        public override ResponseModel Post(DynamicDictionary item)
        {
            return base.Post(item);
        }
        public override ResponseBase Put(int? id, DynamicDictionary item)
        {
            return base.Put(id, item);
        }
    }
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    //public class DonationRequestController:PrivateController
    //{
    //    public bool Post(DynamicDictionary item)
    //    {
    //        bool result = false;
    //        int updt = 0;
    //        int? id = item.GetValueAsInt("id");
    //        int? requested_user_id = item.GetValueAsInt("requested_user_id");
    //        BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
    //        cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format($@"                     
    //             update donation_details set requested_user_id=@requested_user_id where id=@id and is_deleted=false"));
    //        using (DbConnect db = new DbConnect())
    //        {
    //            updt = db.DB.Execute(cmd.FinalSql,new { requested_user_id =requested_user_id,id=id});
    //            if(updt>0)
    //            {
    //                result = true;
    //            }
    //        }
    //        return result;
    //    }
    //}
}