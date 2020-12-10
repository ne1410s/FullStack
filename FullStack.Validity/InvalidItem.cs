// <copyright file="InvalidItem.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Validity
{
    /// <summary>
    /// An invalid item.
    /// </summary>
    public class InvalidItem
    {
        /// <summary>
        /// Gets or sets the navigation.
        /// </summary>
        public string Navigation { get; set; }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public object PropertyValue { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
