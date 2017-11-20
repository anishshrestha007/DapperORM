using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bango
{
    public class ApiRequestHandler : DelegatingHandler
    {
        async protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            response.Headers.Add("X-Shangrila-Header", "System provided by Shangrila Microsystem.");
            if(SessionData.Session != null)
            {
                response.Headers.Add("token", SessionData.token);
                response.Headers.Add("user_id", Base.Conversion.ToString(SessionData.user_id));
            }
            
            //response.Headers.Add("token", request.Headers.GetValues("token").FirstOrDefault<string>());
            //response.Headers.Add("user_id", request.Headers.GetValues("user_id").FirstOrDefault<string>());
            return response;
        }
    }
}
