using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using Bango.FiscalYear;

namespace Bango
{
    public class SessionData
    {
        public static HttpSessionState Session
        {
            get
            {
                //HttpContext.Current
                ////System.Web.Http.HttpC
                //if (HttpContext.Current == null)
                //{
                //    HttpWorkerRequest _wr = new System.Web.Hosting.SimpleWorkerRequest(
                //"/dummyWorkerRequest", Bango.BaseUrl,
                //"default.aspx", null, new StringWriter());
                //}
                if (HttpContext.Current == null)
                    throw new ApplicationException("No Http Context, No Session to Get!");
                //if (HttpContext.Current.Session == null)
                //    return HttpSessionState.
                return HttpContext.Current.Session;
            }
        }

        public static T Get<T>(string key)
        {
            if (Session[key] == null)
                return default(T);
            else
                return (T)Session[key];
        }

        public static void Set<T>(string key, T value)
        {
            Session[key] = value;
        }
        public static string token
        {
            get
            {
                counter = (counter == null ? 0 : counter) + 1;
                return Get<string>("token");
            }
            set { Set<string>("token", value); }
        }
        public static int? user_id
        {
            get { return Get<int?>("user_id"); }
          
            set { Set<int?>("user_id", value); }
        }

        public static string user_name
        {
            get { return Get<string>("user_name"); }

            set { Set<string>("user_name", value); }
        }

        public static int? counter
        {
            get { return Get<int?>("counter"); }
            set { Set<int?>("counter", value); }
        }
        public static int? client_id
        {
            get { return Get<int?>("client_id"); }
            set { Set<int?>("client_id", value); }
        }
        public static DateTime expiry_datetime
        {
            get { return Get<DateTime>("expiry_datetime"); }
            set { Set<DateTime>("expiry_datetime", value); }
        }
        public static string language
        {
            get {
                return Get<string>("language");
            }
            set { Set<string>("language", value); }
        }

        //Get Server Date..
        public static DateTime today_server_date
        {
            get { return Get<DateTime>("today_server_date"); }
            set { Set<DateTime>("today_server_date", value); }
        }

        //FiscalYear Information..
        public static FiscalYear.FiscalYear current_fiscal_year
        {
            get { return new FiscalYear.FiscalYear(current_fiscal_year_code); }
            set { Set<FiscalYear.FiscalYear>("current_fiscal_year", value); }
        }

        public static string current_fiscal_year_code
        {
            get { return Get<string>("current_fiscal_year_code"); }
            set { Set<string>("current_fiscal_year_code", value); }
        }

        public static string current_fiscal_year_id
        {
            get { return Get<string>("current_fiscal_year_id"); }
            set { Set<string>("current_fiscal_year_id", value); }
        }

        public static string current_fiscal_year_start_date_bs
        {
            get { return Get<string>("current_fiscal_year_start_date_bs"); }
            set { Set<string>("current_fiscal_year_start_date_bs", value); }
        }

        public static string current_fiscal_year_end_date_bs
        {
            get { return Get<string>("current_fiscal_year_end_date_bs"); }
            set { Set<string>("current_fiscal_year_end_date_bs", value); }
        }

        public static string photo_path
        {
            get { return Get<string>("photo_path"); }
            set { Set<string>("photo_path", value); }
        }
        public static string client_name
        {
            get { return Get<string>("client_name"); }
            set { Set<string>("client_name", value); }
        }
      
        public static string bank_account_name
        {
            get { return Get<string>("bank_account_name"); }
            set { Set<string>("bank_account_name", value); }
        }
        public static void SetSessionData(string token, int? user_id, string language = null)
        {
            if (language == null)
                language = "np";

            SessionData.token = token;
            SessionData.user_id = user_id;
            SessionData.language = language;
        }
        public static bool LoadSessionDetail()
        {

            //HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            //response.Headers.Add("X-Custom-Header", "This is my custom header.");
            //response.Headers.Add("token", request.Headers.GetValues
            return true;
        }

        //public static
    }
}
