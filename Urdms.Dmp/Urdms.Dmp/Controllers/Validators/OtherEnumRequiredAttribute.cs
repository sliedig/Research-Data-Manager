using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Urdms.Dmp.Controllers.Validators
{
    /// <summary>
    /// This validation attribute compares the targetted property enum value to determine whether the current value must have valid content
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class OtherEnumRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public const string DefaultErrorMessage = "Field is required";
        /// <summary>
        /// Gets the target property of the view model
        /// </summary>
        public string TargetProperty { get; private set; }
        /// <summary>
        /// Gets the enum value which constitutes the other value
        /// </summary>
        public Enum EnumValue { get; private set;}
        
        public OtherEnumRequiredAttribute(string targetProperty, object enumValue) : base(DefaultErrorMessage)
        {
            if (string.IsNullOrWhiteSpace(targetProperty))
            {
                throw new InvalidOperationException("Target Property must be provided");
            }
            if (enumValue == null || !enumValue.GetType().IsEnum)
            {
                throw new InvalidOperationException("Other Value must be an enum");
            }
            this.TargetProperty = targetProperty;
            this.EnumValue = (Enum)enumValue;
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            try
            {
                var info = validationContext.ObjectType.GetProperty(this.TargetProperty,this.EnumValue.GetType());
                if (info != null)
                {
                    var currentValue = (Enum)info.GetValue(validationContext.ObjectInstance, null);
                    var textValue = (string)value;
                    if (!currentValue.HasFlag(this.EnumValue) || !string.IsNullOrWhiteSpace(textValue))
                    {
                        return ValidationResult.Success;
                    }
                }
                

                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            catch 
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "otherEnumRequired"
            };
        }
        
    }
}