using Bango.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Services
{
    public class ServiceUtility
    {
        public static ResponseCollection CaculatePaging(ResponseCollection resp, int total, int page, int pageSize)
        {
            if(resp.success && total > 0)
            {
                resp.total = total;
                int totalpage = (int)Math.Ceiling((double)total / pageSize);
                page = page < 0 ? 0 : page;
                if (totalpage == 0)
                    totalpage = 1;
                int previousPage = (page - 1) == 0 ? page : (page - 1);
                int nextPage = (page + 1) > totalpage ? page : (page + 1);
                resp.page = page;
                resp.totalPages = totalpage;
            }
            return resp;
        }
    }
}
