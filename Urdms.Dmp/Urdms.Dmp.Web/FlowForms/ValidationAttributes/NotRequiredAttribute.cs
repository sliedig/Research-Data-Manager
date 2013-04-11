using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Urdms.Dmp.Web.FlowForms.ValidationAttributes
{
    public class NotRequiredAttribute : ValidationAttribute, IClientValidatable
    {

        public override bool IsValid(object value)
        {
            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule { ValidationType = "notrequired" };
        }
    }
}
