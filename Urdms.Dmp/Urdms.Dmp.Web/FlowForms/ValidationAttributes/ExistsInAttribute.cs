using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Urdms.Dmp.Web.FlowForms.ValidationAttributes
{
    /*
     * Select Lists
     */
    public class ExistsInAttribute : ValidationAttribute, IClientValidatable
    {
        public ExistsInAttribute(string collection):this(collection,null,null){}

        /// <summary>
        /// Ensures field exists in the collection.
        /// </summary>
        /// <param name="collection">Name of IEnumerable in model</param>
        /// <param name="valueKey">Key in collection to validate field against</param>
        /// <param name="displayKey">Key in collection that is displayed</param>
        public ExistsInAttribute(string collection, string valueKey=null, string displayKey=null)
        {
            Collection = collection;
            Value = valueKey;
            Display = displayKey; 
        }

        public string Collection { get; set; }
        public string Value { get; set; }
        public string Display { get; set; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var existsRule = new ModelClientValidationRule { ValidationType = "existsin", ErrorMessage = FormatErrorMessage(metadata.DisplayName ?? metadata.PropertyName) };
            existsRule.ValidationParameters["collection"] = Collection;
            existsRule.ValidationParameters["value"] = Value;
            existsRule.ValidationParameters["display"] = Display;
            yield return existsRule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var model = context.ObjectInstance;
            var collectionProperty = model.GetType().GetProperty(Collection);
            if (collectionProperty == null)
            {
                // Collection from ExistsIn(collection) can't be found
                throw new Exception(String.Format("Model.{0} is null. Unable to make list for Model.{1}", Collection, context.MemberName));
            }
            var collection = (collectionProperty.GetValue(model, null) as IEnumerable).Cast<object>();
            if (value == null)
            {
                // Validating required-ness not implicit in this attribute
                return null;
            }

            var possibleValues = collection.Select(item => item.GetType().GetProperty(Value ?? "Id").GetValue(item, null).ToString());
            if (typeof(IEnumerable).IsAssignableFrom(value.GetType()) && !typeof(string).IsAssignableFrom(value.GetType()))
            {
                if ((value as IEnumerable).Cast<object>().All(v => possibleValues.Any(item => item == v.ToString())))
                {
                    // Each item in object value exists in collection
                    return null;
                }
            }
            else
            {
                if (possibleValues.Any(item => item == value.ToString()))
                {
                    // The object value exists in collection
                    return null;
                }
            }

            var choices = collection.Aggregate(
                new StringBuilder(),
                (sb, item) => (sb.Length == 0 ? sb : sb.Append(", ")).Append(item.GetType().GetProperty(Display ?? "Name").GetValue(item, null))
                );
            ErrorMessage = string.Format("The {0} field must be one of {1}", "{0}", choices);

            return new ValidationResult(FormatErrorMessage(context.DisplayName ?? context.MemberName), new List<string> {Collection});
        }
    }
}