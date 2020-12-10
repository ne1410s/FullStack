// <copyright file="RunTimeExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Db.Extensions.Seed
{
    using System.Linq;
    using FullStack.Db.Abstractions;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Run time extensions for seeding a db context.
    /// </summary>
    public static class RunTimeExtensions
    {
        /// <summary>
        /// Adds any new the entries in the supplied array, according to id.
        /// </summary>
        /// <typeparam name="TEntry">The entry type.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="db">The db context.</param>
        /// <param name="entries">The entries.</param>
        public static void AddById<TEntry, TKey>(this DbContext db, params TEntry[] entries)
            where TEntry : class, IEntry<TKey>
        {
            var dbSet = db.Set<TEntry>();
            var dbIds = dbSet.Select(r => r.Id).ToList();
            var newEntries = entries.Where(n => !dbIds.Contains(n.Id));
            dbSet.AddRange(newEntries);
        }
    }
}
