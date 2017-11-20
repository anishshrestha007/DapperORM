using Bango;
using Bango.Controllers;
using Bango.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Bango.Base.List;

namespace Test.Controllers
{
    public class TotalCountController : PrivateController
    {
        // GET: TotalCount
       public ResponseModel Get()
        {
            ResponseModel resp = new ResponseModel();
            BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
            cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format($@"                     
                 select count(id) as total_donation_details from donation_details"));
            BangoCommand cmd_total_charities = new BangoCommand(MyroCommandTypes.SqlBuilder);
            cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format($@"                     
                 select count(id) as total_charities_events  from charities_events_details"));
            //BangoCommand cmd_total_volunters = new BangoCommand(MyroCommandTypes.SqlBuilder);
            //cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format($@"                     
            //     select count(id) as total_donation_details from donation_details"));
            IEnumerable<dynamic> total_donation_details = null;
            IEnumerable<dynamic> total_charities_event = null;
            using (DbConnect con = new DbConnect())
            {
               total_donation_details = con.DB.Query(cmd.FinalSql.ToString(), null);
                total_charities_event = con.DB.Query(cmd_total_charities.FinalSql.ToString(), null);
            }
            DynamicDictionary dic = new DynamicDictionary();
            dic.Add("total_donation_details", total_donation_details);
            dic.Add("total_donation_details", total_charities_event);
            resp.data = dic;
            return resp;
        }

    }
}