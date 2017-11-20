using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Models.Attributes
{
    public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute
    {   
        //name_np_required
        public RequiredAttribute()
        {
            //get the propery to which the attribute is assigned

            //get lang
        }

        public override bool IsValid(object value)
        {
 
            if (base.IsValid(value) == false)
            {
                return false;
            }
            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //return base.IsValid(value, validationContext);
            if (value != null)
            {
                if (value.ToString().Trim().Length == 0)
                {
                    string[] memberNames = new string[] { validationContext.MemberName };

                    ModelBase model = (ModelBase)validationContext.ObjectInstance;
                    ErrorMessage = model.GetLang(validationContext.MemberName) + " " + model.GetLang("EMPTY_FIELD_ERR_MSG");
                    
                    return new ValidationResult(ErrorMessage, memberNames);
                }
                else
                    return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
