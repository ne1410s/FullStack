// <copyright file="LookupConversionExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Db.Extensions.Seed
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using FullStack.Db.Abstractions;

    /// <summary>
    /// Conversion extensions for lookup entry.
    /// </summary>
    public static class LookupConversionExtensions
    {
        /// <summary>
        /// Gets enum value from a lookup entry.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="entry">The lookup entry.</param>
        /// <returns>An enum value.</returns>
        public static TEnum ToEnum<TEnum>(this ILookupEntry<TEnum> entry)
            where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), entry.Code, true);
        }

        /// <summary>
        /// Gets an enum value from a lookup entry that may be null.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="entry">The lookup entry.</param>
        /// <returns>A nullable enum value.</returns>
        public static TEnum? ToNullableEnum<TEnum>(this ILookupEntry<TEnum> entry)
            where TEnum : struct
        {
            return entry == null ? (TEnum?)null : entry.ToEnum();
        }

        /// <summary>
        /// Gets the integer value of an enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="enumItem">The enum value.</param>
        /// <returns>An integer.</returns>
        public static int ToLookupId<TEnum>(this TEnum enumItem)
           where TEnum : struct
        {
            return (int)Convert.ChangeType(enumItem, typeof(int));
        }

        /// <summary>
        /// Gets the nullable integer value of a nullable enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="enumItem">The nullable enum value.</param>
        /// <returns>A nullable integer.</returns>
        public static int? ToLookupId<TEnum>(this TEnum? enumItem)
           where TEnum : struct
        {
            return enumItem == null ? (int?)null : enumItem.Value.ToLookupId();
        }

        /// <summary>
        /// Populates a basic lookup entry from an enum value alone.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="enumItem">The enum value.</param>
        /// <returns>A lookup entry.</returns>
        public static LookupEntry<TEnum> ToBasicEntry<TEnum>(this TEnum enumItem)
           where TEnum : struct
        {
            var memberInfos = typeof(TEnum).GetMember(enumItem.ToString());
            var keyMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == typeof(TEnum));
            var attribs = keyMemberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            var description = attribs.OfType<DescriptionAttribute>().FirstOrDefault()?.Description;

            return new LookupEntry<TEnum>
            {
                Id = enumItem.ToLookupId(),
                Code = enumItem.ToString(),
                DisplayName = description ?? enumItem.ToString(),
            };
        }

        /// <summary>
        /// Populates a basic lookup entry from a nullable enum value alone.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="enumItem">The nullable enum value.</param>
        /// <returns>A nullable lookup entry.</returns>
        public static LookupEntry<TEnum> ToBasicEntry<TEnum>(this TEnum? enumItem)
           where TEnum : struct
        {
            return enumItem == null ? null : enumItem.Value.ToBasicEntry();
        }
    }
}
