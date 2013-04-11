using System;
using System.Linq;
using System.Web.Mvc;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class FlagEnumerationModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
            {
                var values = GetValue<string[]>(bindingContext, bindingContext.ModelName);

                if (values.Length > 1 && (bindingContext.ModelType.IsEnum && bindingContext.ModelType.IsDefined(typeof(FlagsAttribute), false)))
                {
                    int byteValue = 0;
                    foreach (var value in values.Where(o => Enum.IsDefined(bindingContext.ModelType, o)))
                    {
                        byteValue |= (int)Enum.Parse(bindingContext.ModelType, value);
                    }

                    return Enum.Parse(bindingContext.ModelType, byteValue.ToString());
                }
            }
            return base.BindModel(controllerContext, bindingContext);
        }

        private static T GetValue<T>(ModelBindingContext bindingContext, string key)
        {
            if (bindingContext.ValueProvider.ContainsPrefix(key))
            {
                var valueResult = bindingContext.ValueProvider.GetValue(key);
                if (valueResult != null)
                {
                    bindingContext.ModelState.SetModelValue(key, valueResult);
                    return (T)valueResult.ConvertTo(typeof(T));
                }
            }
            return default(T);
        }
    }
}