// <copyright file="ValidationExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Validity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extensions relating to validation.
    /// </summary>
    public static class ValidationExtensions
    {
        private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Recursively validates the public properties on the provided object.
        /// </summary>
        /// <param name="instance">The provided object.</param>
        /// <param name="errors">A list of errors.</param>
        /// <param name="nav">Property navigation.</param>
        /// <returns>A value indicating whether the object is valid.</returns>
        public static bool Validate(
            this object instance,
            out List<InvalidItem> errors,
            string nav = null)
        {
            errors = new List<InvalidItem>();

            foreach (var prop in instance?.GetType().GetProperties(PublicInstance) ?? Array.Empty<PropertyInfo>())
            {
                var value = prop.GetValue(instance);
                if (prop.PropertyType.IsValueType || prop.PropertyType.IsPrimitive || prop.PropertyType.IsEnum || prop.PropertyType == typeof(string))
                {
                    var tempResults = new List<ValidationResult>();
                    var context = new ValidationContext(instance) { MemberName = prop.Name };
                    Validator.TryValidateProperty(value, context, tempResults);
                    errors.AddRange(tempResults.Select(r => new InvalidItem
                    {
                        ErrorMessage = r.ErrorMessage,
                        Navigation = nav,
                        PropertyName = prop.Name,
                        PropertyValue = value,
                    }));
                }
                else
                {
                    var isArray = prop.PropertyType.IsArray;
                    var items = isArray ? (Array)value : new[] { value };
                    var iteration = 0;
                    var pfx = nav == null ? prop.Name : $"{nav}.{prop.Name}";

                    foreach (var item in items)
                    {
                        var accessor = isArray ? $"[{iteration++}]" : string.Empty;
                        item.Validate(out var propErrors, $"{pfx}{accessor}");
                        errors.AddRange(propErrors);
                    }
                }
            }

            return errors.Count == 0;
        }
    }
}
