// <copyright file="ILookupEntry.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Db.Abstractions
{
    /// <summary>
    /// Contract for a keystore lookup entry.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    public interface ILookupEntry<TEnum> : IEntry<int>
        where TEnum : struct
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        string DisplayName { get; set; }
    }
}
