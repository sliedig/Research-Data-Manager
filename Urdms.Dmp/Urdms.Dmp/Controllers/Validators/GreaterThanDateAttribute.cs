using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace Urdms.Dmp.Controllers.Validators
{
    /// <summary>
    /// To validate two Australian-formatted string dates and ensure the selected date is greater than the other
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class GreaterThanDateAttribute : ValidationAttribute, IClientValidatable
    {
        private const string ErrorMsg = "{0} must be greater than {1}";
        private const string Culture = "en-AU";
        public string StartDateProperty { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanDateAttribute"/> class.
        /// </summary>
        /// <param name="startDateProperty">The start date to compare against.</param>
        public GreaterThanDateAttribute(string startDateProperty)
            : base(ErrorMsg)
        {
            StartDateProperty = startDateProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMsg, name, StartDateProperty);
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var endDate = DateTime.Parse((string)value, CultureInfo.CreateSpecificCulture(Culture));
                var startDate = new DateTime();

                var propertyInfo = validationContext.ObjectType.GetProperty(StartDateProperty);
                if(propertyInfo != null)
                {
                    var secondDateString = propertyInfo.GetValue(validationContext.ObjectInstance, null) as string;
                    startDate = DateTime.Parse(secondDateString, CultureInfo.CreateSpecificCulture(Culture));
                }

                if(endDate > startDate)
                {
                    return ValidationResult.Success;
                }
                
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            catch (Exception)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }

        /// <summary>
        /// When implemented in a class, returns client validation rules for that class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <returns>
        /// The client validation rules for this validator.
        /// </returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
                             {
                                 ErrorMessage = ErrorMsg,
                                 ValidationType = "startDateLessThanEndDate"
                             };
        }
    }
}