// <copyright file="ProviderExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Db.Extensions.Provider
{
    using System;
    using FullStack.Db.Abstractions;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Extensions relating to database provider.
    /// </summary>
    public static class ProviderExtensions
    {
        /// <summary>
        /// The package namespace for
        /// <see cref="Microsoft.EntityFrameworkCore.SqlServer"/>.
        /// </summary>
        public const string EFCORE_SQLSERVER = nameof(Microsoft.EntityFrameworkCore.SqlServer);

        /// <summary>
        /// The package namespace for
        /// <see cref="Microsoft.EntityFrameworkCore.Sqlite"/>.
        /// </summary>
        public const string EFCORE_SQLITE = nameof(Microsoft.EntityFrameworkCore.Sqlite);

        /// <summary>
        /// The package namespace for
        /// <see cref="Pomelo.EntityFrameworkCore.MySql"/>.
        /// </summary>
        public const string EFCORE_MYSQL = nameof(Pomelo.EntityFrameworkCore.MySql);

        /// <summary>
        /// Gets a supported provider type from package namespace.
        /// </summary>
        /// <param name="provider">The package namespace.</param>
        /// <returns>A database type.</returns>
        public static DatabaseType ToProviderType(this string provider)
        {
            return provider switch
            {
                EFCORE_SQLSERVER => DatabaseType.SqlServer,
                EFCORE_SQLITE => DatabaseType.Sqlite,
                EFCORE_MYSQL => DatabaseType.MySql,
                _ => throw new NotSupportedException(
                    $"Provider not supported: {provider}"),
            };
        }

        /// <summary>
        /// Gets db context options for the database type.
        /// </summary>
        /// <param name="dbType">The database type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A set of db context options.</returns>
        public static DbContextOptions ToContextOptions(
            this DatabaseType dbType,
            string connectionString)
        {
            var builder = new DbContextOptionsBuilder();
            return dbType switch
            {
                DatabaseType.MySql => builder.UseMySql(connectionString).Options,
                DatabaseType.Sqlite => builder.UseSqlite(connectionString).Options,
                DatabaseType.SqlServer => builder.UseSqlServer(connectionString).Options,
                _ => throw new NotSupportedException($"Unsupported database type: {dbType}"),
            };
        }
    }
}
