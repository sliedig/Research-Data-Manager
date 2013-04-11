using System;
using System.ComponentModel.DataAnnotations;

namespace Urdms.Dmp.Controllers.Validators
{
    /// <summary>
    /// Useful for validating checkboxes that must be ticked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class BooleanRequiredToBeTrueAttribute : ValidationAttribute
    {
        private const string ErrorMsg = "This field must be checked to proceed";

        public BooleanRequiredToBeTrueAttribute() : base(ErrorMsg) {}

        public override bool IsValid(object value)
        {
            return value != null && (bool)value;
        }
    }
}