// <copyright file="DesignTimeExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Db.Extensions.Seed
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Design time extensions for seeding a model builder.
    /// </summary>
    public static class DesignTimeExtensions
    {
        /// <summary>
        /// Upserts all enum options to its lookup table.
        /// </summary>
        /// <typeparam name="TLookup">The lookup type.</typeparam>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="modelBuilder">The model builder.</param>
        public static void SeedEnum<TLookup, TEnum>(this ModelBuilder modelBuilder)
            where TLookup : LookupEntry<TEnum>
            where TEnum : struct
        {
            var lookupEntries = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(o => o.ToBasicEntry())
                .ToList();

            modelBuilder.Entity<TLookup>()
                .HasAlternateKey(r => r.Code);

            modelBuilder.Entity<TLookup>()
                .HasData(lookupEntries);
        }
    }
}
