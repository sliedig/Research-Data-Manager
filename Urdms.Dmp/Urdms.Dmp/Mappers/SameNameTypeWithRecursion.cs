using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Omu.ValueInjecter;

namespace Urdms.Dmp.Mappers
{
    internal class SameTypeWithExclusions : ValueInjection
    {
        protected IEnumerable<string> ExcludedProperties { get; private set; }

        public SameTypeWithExclusions(IEnumerable<string> excludedProperties)
        {
            ExcludedProperties = excludedProperties ?? Enumerable.Empty<string>();
        }

        protected override void Inject(object source, object target)
        {
            if (source == null || target == null)
                return;

            var props = source.GetProps();
            for (var i = 0; i < props.Count; i++)
            {
                var descriptor = props[i];
                
                if (ExcludedProperties.Any(o => o == descriptor.Name)) continue;
                
                var byName = target.GetProps().GetByName(descriptor.Name);

                if (byName == null) continue;

                var descriptorValue = descriptor.GetValue(source);

                if (descriptor.PropertyType == typeof(string))
                {
                    byName.SetValue(target, descriptorValue);
                }
                else if (descriptor.PropertyType == byName.PropertyType)
                {
                    byName.SetValue(target, descriptorValue);
                }
            }
        }
    }
    
    internal class SameNameTypeWithRecursion : ValueInjection
    {
        protected IEnumerable<string> ExcludedProperties { get; private set; }

        public SameNameTypeWithRecursion() : this(null) {}

        public SameNameTypeWithRecursion(IEnumerable<string> excludedProperties)
        {
            ExcludedProperties = excludedProperties ?? Enumerable.Empty<string>();
        }

        protected override void Inject(object source, object target)
        {
            if (source == null || target == null)
                return;
                
            var props = source.GetProps();
            for (var i = 0; i < props.Count; i++)
            {
                var descriptor = props[i];

                if (ExcludedProperties.Any(o => o == descriptor.Name)) continue;
                
                var byName = target.GetProps().GetByName(descriptor.Name);

                if (byName == null) continue;

                var descriptorValue = descriptor.GetValue(source);

                if (descriptor.PropertyType == typeof(string))
                {
                    byName.SetValue(target, descriptorValue);
                }
                else if (descriptor.PropertyType.IsClass)
                {
                    Inject(descriptorValue,byName.GetValue(target));
                }
                else if (descriptor.PropertyType == byName.PropertyType)
                {
                    byName.SetValue(target, descriptorValue);
                }
            }
        }
    }

    internal class SameNameWithRecursion : ValueInjection
    {
        protected IEnumerable<string> ExcludedProperties { get; private set; }

        public SameNameWithRecursion() : this(null) {}

        public SameNameWithRecursion(IEnumerable<string> excludedProperties)
        {
            ExcludedProperties = excludedProperties ?? Enumerable.Empty<string>();
        }

        protected override void Inject(object source, object target)
        {
            if (source == null || target == null)
                return;

            var props = source.GetProps();
            for (var i = 0; i < props.Count; i++)
            {
                var descriptor = props[i];

                if (ExcludedProperties.Any(o => o == descriptor.Name)) continue;

                var byName = target.GetProps().GetByName(descriptor.Name);

                if (byName == null) continue;

                var descriptorValue = descriptor.GetValue(source);

                if (descriptor.PropertyType == typeof(string))
                {
                    if (byName.PropertyType.IsGenericType && byName.PropertyType.GetGenericArguments()[0] == typeof(DateTime))
                    {
                        DateTime result;
                        var text = (string) descriptorValue ?? "";
                        if (DateTime.TryParse(text,out result))
                        {
                            byName.SetValue(target,result);
                        }
                        else
                        {
                            byName.SetValue(target,null);
                        }
                    }
                    else
                    {
                        byName.SetValue(target, descriptorValue);
                    }
                    
                }
                else if (descriptor.PropertyType.IsClass)
                {
                    if (descriptorValue != null)
                    {
                        OnTypeIsClass(descriptor, byName, source, target, descriptorValue, byName.GetValue(target));
                    }
                }
                else if (descriptor.PropertyType == byName.PropertyType)
                {
                    byName.SetValue(target, descriptorValue);
                }
            }
        }

        protected virtual void OnTypeIsClass(PropertyDescriptor sourceDescriptor, PropertyDescriptor targetDescriptor, object sourceInstance, object targetInstance, object sourceValue, object targetValue)
        {
            Inject(sourceValue, targetValue);
        }

    }

    internal class SameNameWithRecursionAndTargetInstantiationIfRequired : SameNameWithRecursion
    {
        protected override void OnTypeIsClass(PropertyDescriptor sourceDescriptor, PropertyDescriptor targetDescriptor, object sourceInstance, object targetInstance, object sourceValue, object targetValue)
        {
            if (targetValue == null && !targetDescriptor.IsReadOnly)
            {
                targetValue = Activator.CreateInstance(targetDescriptor.PropertyType);
                // injects new instance into the target object
                // which then allows the injection of values from the source object to the target object
                targetDescriptor.SetValue(targetInstance, targetValue);
            }
            Inject(sourceValue, targetValue);
        }
    }
}