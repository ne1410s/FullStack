// <copyright file="IEntry.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Db.Abstractions
{
    /// <summary>
    /// Contract for a keystore entry.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    public interface IEntry<TKey>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public TKey Id { get; set; }
    }
}
