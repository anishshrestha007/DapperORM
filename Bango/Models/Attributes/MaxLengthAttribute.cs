using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Models.Attributes
{
    public class MaxLengthAttribute :System.ComponentModel.DataAnnotations.MaxLengthAttribute
    {
        int MaxLength = 0;
        public MaxLengthAttribute(int length)
        {
            MaxLength = length;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            if (value.ToString().Length > MaxLength)
                return false;

            return true;
        }
        public override bool RequiresValidationContext
        {
            get
            {
                return base.RequiresValidationContext;
            }
        }


        public override string FormatErrorMessage(string name)
        {
            ModelBase model = new ModelBase();
            ErrorMessage = model.GetLang(name) + " " + model.GetLang("max_length_error_msg");

            return ErrorMessage; // base.FormatErrorMessage(name);
        }


    }
}
