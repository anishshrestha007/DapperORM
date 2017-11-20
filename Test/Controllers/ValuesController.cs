using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Test.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public void Get()
        {
            using (var client = new HttpClient())
            {
                dynamic data = new ExpandoObject();
                data.tokens = new List<string>() { "" };
                data.notification = new ExpandoObject() as dynamic;
                data.notification.alert = "message";

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                client.BaseAddress = new Uri("https://api.ionic.io/push/notifications");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("X-Ionic-Application-Id", "12312313236262323");
                var keyBase64 = "Basic " + "AAAADdyGb7M:APA91bFVS5SCkDEBRQNtZsxxCXjbu-FgNu0xTtnHwP_z4IecsGq1Lul_IJ3rdwpNVugkmQJoeh4iJ6wH8n-Hh7iU0mUwLW82x-i8ljvIvbbUOT0zm_THIa0pdIheOx-0aSqjuS-7vzbG";
                client.DefaultRequestHeaders.Add("Authorization", keyBase64);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.ionic.io/push/notifications");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.SendAsync(request).Result;
            }
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
