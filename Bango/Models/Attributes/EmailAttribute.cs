using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bango.Models.Attributes
{
    public class EmailAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult  result = null;
            if (!new Regex(@"^[\w-\.]{1,}\@([\w]{1,}\.){1,}[a-z]{2,4}$").IsMatch(value.ToString()))
            {
                string[] memberNames = new string[] { validationContext.MemberName };

                ModelBase model = (ModelBase)validationContext.ObjectInstance;
                string  errormessage = model.GetLang(validationContext.MemberName) + " " + model.GetLang("Email_error_msg");
                ErrorMessage = errormessage;

                return new ValidationResult(errormessage, memberNames);
            }

            return result;
        }
        public override bool IsValid(object value)
        {
            return base.IsValid(value);
        }

        public override bool IsDefaultAttribute()
        {
            return base.IsDefaultAttribute();
        }
    }
}
