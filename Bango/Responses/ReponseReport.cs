using Bango.Base.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Responses
{
    public class ReponseReport : ResponseBase, Bango.Responses.IResponseReport
    {
        public ReponseReport()
            : this(false, string.Empty)
        {

        }
        public ReponseReport(bool success, string report_url, string report_name)
            : this(success, report_url, report_name, "", string.Empty)
        {
        }

        public ReponseReport(bool success, string report_url, string report_name, string message)
            : this(success, report_url, report_name, message, string.Empty)
        {
        }
        public ReponseReport(bool success, string message)
            : this(success, null, message, string.Empty)
        {

        }
        public ReponseReport(bool success, string report_url, string report_name, string message, string errorCode)
            : base(success, message, errorCode)
        {
            this.report_url = report_url;
            this.report_name = report_name;
        }
        /*
        public ReponseReport(bool success, object data, string message, string errorCode, DynamicDictionary validation_errors)
            : base(success, message, errorCode)
        {
            this.data = data;
            this.validation_errors = validation_errors;
        }
        */

        public DynamicDictionary PushValidationErrors(DynamicDictionary errorList)
        {
            return null;
            //validation_errors = Models.ModelService.PushValidationErros(errorList, validation_errors);
            //return validation_errors;
        }
        public string report_url { get; set; }
        public string report_name { get; set; }
        public bool isData { get; set; } = true;
    }
}
