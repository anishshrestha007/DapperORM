using Bango.Base.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Responses
{
    public class ResponseModel : ResponseBase, Bango.Responses.IResponseModel
    {
        public ResponseModel()
            : this(false, string.Empty)
        {

        }
        public ResponseModel(bool success, object data)
            : this(success, data, "", string.Empty)
        {
        }

        public ResponseModel(bool success, object data, string message)
            : this(success, data, message, string.Empty)
        {
        }
        public ResponseModel(bool success, string message)
            : this(success, null, message, string.Empty)
        {

        }
        public ResponseModel(bool success, object data, string message, string errorCode)
            : base(success, message, errorCode)
        {
            this.data = data;
        }

        public ResponseModel(bool success, object data, string message, string errorCode, DynamicDictionary validation_errors)
            : base(success, message, errorCode)
        {
            this.data = data;
            this.validation_errors = validation_errors;
        }
        public DynamicDictionary validation_errors { get; set; } = new DynamicDictionary();

        public DynamicDictionary PushValidationErrors(DynamicDictionary errorList)
        {
            validation_errors = Models.ModelService.PushValidationErros(errorList, validation_errors);
            return validation_errors;
        }
        public object data { get; set; }
    }
}
