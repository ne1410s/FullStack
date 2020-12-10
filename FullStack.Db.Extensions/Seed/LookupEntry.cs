// <copyright file="LookupEntry.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Db.Extensions.Seed
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using FullStack.Db.Abstractions;

    /// <summary>
    /// A keystore lookup entry.
    /// </summary>
    /// <typeparam name="TEnum">The key type.</typeparam>
    public class LookupEntry<TEnum> : ILookupEntry<TEnum>
        where TEnum : struct
    {
        /// <inheritdoc/>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <inheritdoc/>
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <inheritdoc/>
        [MaxLength(100)]
        public string DisplayName { get; set; }
    }
}
