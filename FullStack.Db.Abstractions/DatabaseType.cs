// <copyright file="DatabaseType.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Db.Abstractions
{
    /// <summary>
    /// Supported database type.
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// MS SQL Server.
        /// </summary>
        SqlServer,

        /// <summary>
        /// Sqlite.
        /// </summary>
        Sqlite,

        /// <summary>
        /// Oracle MySQL.
        /// </summary>
        MySql,
    }
}
