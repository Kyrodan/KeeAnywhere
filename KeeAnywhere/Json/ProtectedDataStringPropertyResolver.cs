using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KeeAnywhere.Json
{
    internal class ProtectedDataStringPropertyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);

            // Find all string properties that have a [JsonEncrypt] attribute applied
            // and attach an ProtectedDataStringValueProvider instance to them
            foreach (var prop in props.Where(p => p.PropertyType == typeof(string)))
            {
                var pi = type.GetProperty(prop.UnderlyingName);
                if (pi != null && pi.GetCustomAttribute(typeof(JsonEncryptAttribute), true) != null)
                {
                    prop.ValueProvider =
                        new ProtectedDataStringValueProvider(pi);
                }
            }

            return props;
        }

        internal class ProtectedDataStringValueProvider : IValueProvider
        {
            private readonly PropertyInfo _targetProperty;

            public ProtectedDataStringValueProvider(PropertyInfo targetProperty)
            {
                if (targetProperty == null) throw new ArgumentNullException("targetProperty");

                this._targetProperty = targetProperty;
            }

            public void SetValue(object target, object value)
            {
                var encrypted = Convert.FromBase64String((string)value);
                var buffer = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);

                var decryptedValue = Encoding.UTF8.GetString(buffer);
                _targetProperty.SetValue(target, decryptedValue);
            }

            public object GetValue(object target)
            {
                var value = (string)_targetProperty.GetValue(target);
                var buffer = Encoding.UTF8.GetBytes(value);
                var encrypted = ProtectedData.Protect(buffer, null, DataProtectionScope.CurrentUser);

                return Convert.ToBase64String(encrypted);
            }
        }
    }
}